namespace CraftDay.ToDo.FSharpRop

open CraftDay.ToDo.Common.Dto
open CraftDay.ToDo.Common.Services
open System.Collections

type ToDoFSharpRopService(store: IToDoStore) =
  interface IToDoService with
    override this.GetToDoItems(): Generic.List<ToDoItem> =
      store.GetAllItems()