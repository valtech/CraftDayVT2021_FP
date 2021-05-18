using System.Collections.Generic;
using CraftDay.ToDo.Common.Dto;

namespace CraftDay.ToDo.Common.Services
{
  public interface IToDoStore
  {
    public ToDoItem GetItem(int id);
    public List<ToDoItem> GetAllItems();
    public void SetItem(int id, ToDoItem item);
  }
}