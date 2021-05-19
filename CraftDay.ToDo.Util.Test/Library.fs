namespace CraftDay.ToDo.Util.Test

open CraftDay.ToDo.Common.Dto
open CraftDay.ToDo.Common.Services

module Utils =
  type DummyToDoStore() =
    let mutable store: Map<int, ToDoItem> = Map.empty 
    interface IToDoStore with
      override _.GetItem id =
        match store.TryFind(id) with
        | Some t -> t
        | None -> null
      
      override _.SetItem (id, item) =
        store <- store.Add(id, item)

      member this.GetAllItems() =
        store
        |> Map.fold (fun acc _k v -> v :: acc) []
        |> ResizeArray
    member this.AddAll(todos: (int * ToDoItem) list) =
      let todo = this :> IToDoStore
      todos
      |> List.iter todo.SetItem