using CraftDay.ToDo.Common.Dto;

namespace CraftDay.ToDo.Common.Services
{
  public interface IToDoStore
  {
    public ToDoItem GetItem(string id);
    public void SetItem(string id, ToDoItem item);
  }
}