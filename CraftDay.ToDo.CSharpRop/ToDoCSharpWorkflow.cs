using System;
using System.Collections.Generic;
using System.Net;
using CraftDay.ToDo.Common.Dto;
using CraftDay.ToDo.Common.Services;
using CraftDay.ToDo.CSharp.Errors;
using CraftDay.ToDo.CSharpRop.Validators;
using LanguageExt;
using Newtonsoft.Json;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace CraftDay.ToDo.CSharpRop
{
  public class ToDoCSharpWorkflow
  {
    private readonly ToDoCSharpRopService _service;

    public ToDoCSharpWorkflow(IToDoStore store)
    {
      _service = new ToDoCSharpRopService(store);
    }

    private static Either<Exception, int> ConvertToInt(NonEmptyString str)
      => new Try<int>(() => int.Parse(str.Value()))
        .ToEither()
        .MapLeft(e => (Exception) new ValidationException(e.Message));
    
    private Either<Exception, ToDoItem> GetItemFromStore(int id)
      => new Try<ToDoItem>(() => _service.GetItem(id))
        .ToEither()
        .MapLeft(e => (Exception) new DomainException(e.Message));
    
    private Either<Exception, ToDoItem> SetItemToDoneInStore(int id, bool isDone)
      => new Try<ToDoItem>(() =>
        {
          var item = _service.GetItem(id);
          item.IsDone = isDone;
          _service.SetItem(id, item);
          return item;
        })
        .ToEither()
        .MapLeft(e => (Exception) new DomainException(e.Message));

    private Either<Exception, ToDoItem> SetNameInStore(int id, string name)
      => new Try<ToDoItem>(() =>
        {
          var item = _service.GetItem(id);
          item.Name = name;
          _service.SetItem(id, item);
          return item;
        })
        .ToEither()
        .MapLeft(e => (Exception) new DomainException(e.Message));
    
    private Either<Exception, List<ToDoItem>> GetAllItemsFromStore()
      => new Try<List<ToDoItem>>(() => _service.GetAllItems())
        .ToEither()
        .MapLeft(e => (Exception) new DomainException(e.Message));
    
    private static ToDoGetItemsMessage WrapToDoInEnvelope(ToDoItem item)
      => new ToDoGetItemsMessage {items = new List<ToDoItem> {item}};
    
    private static ToDoGetItemsMessage WrapToDoListInEnvelope(List<ToDoItem> items)
      => new ToDoGetItemsMessage {items = items};

    private static Tuple<HttpStatusCode, string> HandleHttpError(Exception err)
      => err switch {
        ValidationException e => Tuple(HttpStatusCode.BadRequest, e.ToString()),
        DomainException e => Tuple(HttpStatusCode.BadRequest, e.ToString()),
        DaoException => Tuple(
          HttpStatusCode.InternalServerError,
          JsonConvert.SerializeObject(new {type = "InternalError", error = "Internal error"})),
        _ => Tuple(
          HttpStatusCode.InternalServerError,
          JsonConvert.SerializeObject(new {type = "InternalError", error = "Internal error"}))
      };
    
    public Tuple<HttpStatusCode, string>  GetAllItems()
     => GetAllItemsFromStore()
        .Map<ToDoGetItemsMessage>(WrapToDoListInEnvelope) // Get results
        .Map<string>(JsonConvert.SerializeObject) // Serialize
        .Match(
          val => Tuple(HttpStatusCode.OK, val), 
          HandleHttpError);

    public Tuple<HttpStatusCode, string>  GetItem(string param)
     => NonEmptyString
        .From(param) // Deserialize
        .Bind<int>(ConvertToInt) // Validate input
        .Bind<ToDoItem>(GetItemFromStore) // Get data from store
        .Map<ToDoGetItemsMessage>(WrapToDoInEnvelope) // Get results
        .Map<string>(JsonConvert.SerializeObject) // Serialize
        .Match(
          val => Tuple(HttpStatusCode.OK, val), 
          HandleHttpError);

    private
      Validation<ValidationException, Tuple<int, bool>>
      ValidateSetStatus(string pathParam, string body)
    {
      var id = 
        NonEmptyString
        .From(pathParam) // Deserialize
        .Bind<int>(ConvertToInt)
        .Match(
          i => Success<ValidationException, int>(i),
          e => Fail<ValidationException, int>((ValidationException)e)
          );
      var status =
        new Try<SetIsDone>(() => JsonConvert.DeserializeObject<SetIsDone>(body))
          .ToEither()
          .Match(
            isDone => Success<ValidationException, bool>(isDone.IsDone),
            e => Fail<ValidationException, bool>((ValidationException)e)
          );

      var result = 
        from x in id
        from y in status
        select Tuple(x, y);
      
      return result;
    }
    
    private
      Validation<ValidationException, Tuple<int, string>>
      ValidateSetName(string pathParam, string body)
    {
      var id = 
        NonEmptyString
          .From(pathParam) // Deserialize
          .Bind<int>(ConvertToInt)
          .Match(
            i => Success<ValidationException, int>(i),
            e => Fail<ValidationException, int>((ValidationException)e)
          );
      var name =
        new Try<SetName>(() => JsonConvert.DeserializeObject<SetName>(body))
          .ToEither()
          .Match(
            setName => Success<ValidationException, string>(setName.Name),
            e => Fail<ValidationException, string>((ValidationException)e)
          );

      var result = 
        from x in id
        from y in name
        select Tuple(x, y);
      
      return result;
    }
    
    public Tuple<HttpStatusCode, string> SetIsDone(string pathParam, string body)
      => ValidateSetStatus(pathParam, body) // Validate input
        .ToEither() // Convert the error type Validation -> Either
        .MapLeft(e => (Exception) e.Head) // Just pick the first error
        .Bind<ToDoItem>(t => SetItemToDoneInStore(t.Item1, t.Item2)) // Get data from store
        .Map<ToDoGetItemsMessage>(WrapToDoInEnvelope) // Get results
        .Map<string>(JsonConvert.SerializeObject) // Serialize
        .Match(
          val => Tuple(HttpStatusCode.OK, val), 
          HandleHttpError);
    
    public Tuple<HttpStatusCode, string> SetName(string pathParam, string body)
      => ValidateSetName(pathParam, body) // Validate input
        .ToEither() // Convert the error type Validation -> Either
        .MapLeft(e => (Exception) e.Head) // Just pick the first error
        .Bind<ToDoItem>(t => SetNameInStore(t.Item1, t.Item2)) // Get data from store
        .Map<ToDoGetItemsMessage>(WrapToDoInEnvelope) // Get results
        .Map<string>(JsonConvert.SerializeObject) // Serialize
        .Match(
          val => Tuple(HttpStatusCode.OK, val), 
          HandleHttpError);
  }
}