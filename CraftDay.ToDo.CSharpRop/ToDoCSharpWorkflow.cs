using System;
using System.Collections.Generic;
using System.Net;
using CraftDay.ToDo.Common.Dto;
using CraftDay.ToDo.Common.Services;
using CraftDay.ToDo.CSharp.Errors;
using CraftDay.ToDo.CSharpRop.Validators;
using LanguageExt;
using Newtonsoft.Json;

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
        ValidationException e => Tuple.Create(HttpStatusCode.BadRequest, e.ToString()),
        DomainException e => Tuple.Create(HttpStatusCode.BadRequest, e.ToString()),
        DaoException => Tuple.Create(
          HttpStatusCode.InternalServerError,
          JsonConvert.SerializeObject(new {type = "InternalError", error = "Internal error"})),
        _ => Tuple.Create(
          HttpStatusCode.InternalServerError,
          JsonConvert.SerializeObject(new {type = "InternalError", error = "Internal error"}))
      };
    
    public Tuple<HttpStatusCode, string>  GetAllItems()
     => GetAllItemsFromStore()
        .Map<ToDoGetItemsMessage>(WrapToDoListInEnvelope) // Get results
        .Map<string>(JsonConvert.SerializeObject) // Serialize
        .Match(
          val => Tuple.Create(HttpStatusCode.OK, val), 
          HandleHttpError);

    public Tuple<HttpStatusCode, string>  GetItem(string param)
     => NonEmptyString
        .From(param) // Deserialize
        .Bind<int>(ConvertToInt) // Validate input
        .Bind<ToDoItem>(GetItemFromStore) // Get data from store
        .Map<ToDoGetItemsMessage>(WrapToDoInEnvelope) // Get results
        .Map<string>(JsonConvert.SerializeObject) // Serialize
        .Match(
          val => Tuple.Create(HttpStatusCode.OK, val), 
          HandleHttpError);
  }
}