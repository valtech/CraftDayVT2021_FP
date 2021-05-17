using System.Collections.Generic;
using CraftDay.ToDo.Common.Dto;

namespace CraftDay.ToDo.Common.Services
{
  public interface IToDoStore
  {
    public ToDoItem GetItem(string id);
    public List<ToDoItem> GetAllItems();
    public void SetItem(string id, ToDoItem item);
  }
}