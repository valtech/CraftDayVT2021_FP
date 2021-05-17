using System.Collections.Generic;
using CraftDay.ToDo.Common.Dto;
using CraftDay.ToDo.Common.Services;

namespace CraftDay.ToDo.CSharp
{
  public class ToDoCSharpService : IToDoService
  {
    private readonly IToDoStore _store;

    public ToDoCSharpService(IToDoStore store)
    {
      _store = store;
    }

    public List<ToDoItem> GetToDoItems()
    {
      return _store.GetAllItems();
    }
  }
}