using System.Collections.Generic;

namespace CraftDay.ToDo.Common.Dto
{
  public class ToDoGetItemsMessage
  {
    public List<ToDoItem> items { get; set; }
  }
}