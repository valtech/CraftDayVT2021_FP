module CraftDay.ToDo.Unit.Test

open CraftDay.ToDo.Common.Dto
open CraftDay.ToDo.Common.Services
open CraftDay.ToDo.CSharp
open CraftDay.ToDo.CSharpRop
open CraftDay.ToDo.FSharpRop
open NUnit.Framework

type DummyToDoStore(todos: (int * ToDoItem) list) =
  interface IToDoStore with
    override _.GetItem id =
      failwith "pang"
    override _.SetItem (id, item) =
      failwith "pang"

[<TestFixture>]
type TodoTests() =
  
  //[<SetUp>]
  static member Services() = [|
    ToDoCSharpFactory() :> ToDoServiceFactory
    ToDoCSharpRopFactory() :> ToDoServiceFactory
    ToDoFSharpRopFactory() :> ToDoServiceFactory
  |]

  [<TestCaseSource("Services")>]
  member _.``Can view TODO list`` (serviceFactory: ToDoServiceFactory) =
      // Arrange
      let todoA = ToDoItem (TaskDescription = "TODO A")
      let todoB = ToDoItem (TaskDescription = "TODO B")
      let todoStore = DummyToDoStore([(1, todoA); (2, todoB)])
      let service = serviceFactory.withStore(todoStore).Build()
      
      // Act
      let actual = service.GetToDoItems()
      
      // Assert
      CollectionAssert.Contains([todoA; todoB], actual)
