using System;
using System.Collections.Generic;
using CraftDay.ToDo.Common.Dto;
using CraftDay.ToDo.Common.Services;

namespace CraftDay.ToDo.CSharpRop
{
  public class ToDoCSharpRopService : IToDoService
  {
    private readonly IToDoStore _store;

    public ToDoCSharpRopService(IToDoStore store)
    {
      _store = store;
    }

    public List<ToDoItem> GetAllItems()
    {
      return _store.GetAllItems();
    }

    public ToDoItem GetItem(int id)
    {
      return _store.GetItem(id);
    }
  }
}