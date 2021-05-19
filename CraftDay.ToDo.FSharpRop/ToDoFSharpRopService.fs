namespace CraftDay.ToDo.FSharpRop

open CraftDay.ToDo.Common.Dto
open CraftDay.ToDo.Common.Services
open System.Collections

type ToDoFSharpRopService(store: IToDoStore) =
  interface IToDoService with
    override this.GetAllItems(): Generic.List<ToDoItem> =
      store.GetAllItems()
      
    override this.GetItem id: ToDoItem =
      store.GetItem id
      
    override this.SetItem(id, item): unit =
      store.SetItem(id, item)