using System;
using System.Collections.Generic;
using CraftDay.ToDo.Common.Dto;
using CraftDay.ToDo.Common.Services;
using CraftDay.ToDo.CSharp.Errors;

namespace CraftDay.ToDo.CSharp
{
  public class ToDoCSharpService : IToDoService
  {
    private readonly IToDoStore _store;

    public ToDoCSharpService(IToDoStore store)
    {
      _store = store;
    }

    public List<ToDoItem> GetAllItems()
    {
      try { return _store.GetAllItems(); } catch (Exception e)
      {
        Console.Out.WriteLine(e.Message);
        throw new DaoException("Error reading objects from store");
      }
    }

    public ToDoItem GetItem(int id)
    {
      try{
        return _store.GetItem(id);
      } catch (KeyNotFoundException e)
      {
        Console.Out.WriteLine(e.Message);
        throw new DomainException($"Could not find item {id} in store");
      } catch (Exception e)
      {
        Console.Out.WriteLine(e.Message);
        throw new DaoException($"Error reading item {id} from store");
      }
    }
  }
}