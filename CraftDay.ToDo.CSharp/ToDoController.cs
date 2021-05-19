using System;
using System.Collections.Generic;
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
    
    public string GetAllItems()
    {
      try
      {
        // Call service from domain
        var items = _service.GetAllItems();
        
        // Package to return type
        var envelope = new ToDoGetItemsMessage{items = items};
        
        // Serialize result
        return JsonConvert.SerializeObject(envelope);
        
      } catch (ValidationException e) {
        return e.ToString();
      } catch (DomainException e) {
        return e.ToString();
      } catch (Exception) {
        return JsonConvert.SerializeObject(new {type = "InternalError", error = "Internal error"});
      }
    }
    
    public string GetItem(string id)
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
        return JsonConvert.SerializeObject(envelope);
        
      } catch (ValidationException e) {
        return e.ToString();
      } catch (DomainException e) {
        return e.ToString();
      } catch (Exception) {
        return JsonConvert.SerializeObject(new {type = "InternalError", error = "Internal error"});
      }
    }
  }
}