using System.Collections.Generic;
using CraftDay.ToDo.Common.Dto;

namespace CraftDay.ToDo.Common.Services
{
    public interface IToDoService
    {
        public ICollection<ToDoItem> GetToDoItems();
    }
}
