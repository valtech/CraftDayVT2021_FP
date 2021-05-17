module CraftDay.ToDo.Unit.Test

open System.Collections.Generic
open CraftDay.ToDo.Common.Dto
open CraftDay.ToDo.Common.Services
open CraftDay.ToDo.CSharp
open CraftDay.ToDo.CSharpRop
open CraftDay.ToDo.FSharpRop
open NUnit.Framework

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
    

[<TestFixture>]
type TodoTests() =
  
  static member Services: ToDoServiceFactory list = [
    ToDoCSharpFactory() 
    ToDoCSharpRopFactory()
    ToDoFSharpRopFactory()
  ]
  
  [<TestCaseSource("Services")>]
  member _.``Can view TODO list`` (serviceFactory: ToDoServiceFactory) =
    // Arrange
    let todoA = ToDoItem (TaskDescription = "TODO A")
    let todoB = ToDoItem (TaskDescription = "TODO B")
    let todoStore = DummyToDoStore()
    todoStore.AddAll([(1, todoA); (2, todoB)])
    let factory = serviceFactory//ToDoCSharpFactory() 
    let service = factory.WithStore(todoStore).Build()
    
    // Act
    let actual = service.GetToDoItems()
    
    // Assert
    Assert.AreEqual(2, actual.Count)
    Assert.Contains(todoA, actual)
    Assert.Contains(todoB, actual)
