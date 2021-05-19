using System;
using System.Collections.Generic;
using System.Net;
using CraftDay.ToDo.Common.Dto;
using CraftDay.ToDo.CSharp.Errors;
using Newtonsoft.Json;

namespace CraftDay.ToDo.CSharp
{
  public class ToDoController
  {
    private readonly ToDoCSharpService _service;

    public ToDoController(ToDoCSharpService service)
    {
      _service = service;
    }
    
    public Tuple<HttpStatusCode, string> GetAllItems()
    {
      try
      {
        // Call service from domain
        var items = _service.GetAllItems();
        
        // Package to return type
        var envelope = new ToDoGetItemsMessage{items = items};
        
        // Serialize result
        return Tuple.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(envelope));
        
      } catch (ValidationException e) {
        return Tuple.Create(HttpStatusCode.BadRequest, e.ToString());
      } catch (DaoException) {
        return Tuple.Create(
          HttpStatusCode.InternalServerError, 
          JsonConvert.SerializeObject(new {type = "InternalError", error = "Internal error"}));
      } catch (Exception) {
        return 
          Tuple.Create(
            HttpStatusCode.InternalServerError, 
            JsonConvert.SerializeObject(new {type = "InternalError", error = "Internal error"}));
      }
    }
    
    public Tuple<HttpStatusCode, string>  GetItem(string id)
    {
      try
      {
        // Parse input
        var itemId = int.Parse(id);
        
        // Call service from domain
        var item = _service.GetItem(itemId);
        
        // Package to return type
        var items = new List<ToDoItem> { item };
        var envelope = new ToDoGetItemsMessage{ items = items };
        
        // Serialize result
        return Tuple.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(envelope));
        
      } catch (ValidationException e) {
        return Tuple.Create(HttpStatusCode.BadRequest, e.ToString());
      } catch (DaoException) {
        return Tuple.Create(
          HttpStatusCode.InternalServerError, 
          JsonConvert.SerializeObject(new {type = "InternalError", error = "Internal error"}));
      } catch (Exception) {
        return 
          Tuple.Create(
            HttpStatusCode.InternalServerError, 
            JsonConvert.SerializeObject(new {type = "InternalError", error = "Internal error"}));
      }
    }
    
    public Tuple<HttpStatusCode, string> SetIsDone(string id, string body)
    {
      try
      {
        // Parse input
        var itemId = int.Parse(id);
        var setIsDone = JsonConvert.DeserializeObject<SetIsDone>(body);
        
        // Call service from domain
        var item = _service.GetItem(itemId);
        item.IsDone = setIsDone.IsDone;
        _service.SetItem(itemId, item);
        
        // Package to return type
        var items = new List<ToDoItem> { item };
        var envelope = new ToDoGetItemsMessage{ items = items };
        
        // Serialize result
        return Tuple.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(envelope));
        
      } catch (ValidationException e) {
        return Tuple.Create(HttpStatusCode.BadRequest, e.ToString());
      } catch (DaoException) {
        return Tuple.Create(
          HttpStatusCode.InternalServerError, 
          JsonConvert.SerializeObject(new {type = "InternalError", error = "Internal error"}));
      } catch (Exception) {
        return 
          Tuple.Create(
            HttpStatusCode.InternalServerError, 
            JsonConvert.SerializeObject(new {type = "InternalError", error = "Internal error"}));
      }
    }
  }
}