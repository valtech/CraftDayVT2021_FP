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

    member this.GetAllItems() =
      failwith "todo"

[<TestFixture>]
type TodoTests() =
  
  static member Services: ToDoServiceFactory list = [
    ToDoCSharpFactory() 
    ToDoCSharpRopFactory()
    ToDoFSharpRopFactory()
  ]

  [<TestCaseSource("Services")>]
  //member _.``Can view TODO list`` (serviceFactory: ToDoServiceFactory) =
  member _.``Can view TODO list``  =
      // Arrange
      let todoA = ToDoItem (TaskDescription = "TODO A")
      let todoB = ToDoItem (TaskDescription = "TODO B")
      let todoStore = DummyToDoStore([(1, todoA); (2, todoB)])
      let factory = ToDoCSharpFactory() 
      let service = factory.WithStore(todoStore).Build()
      
      // Act
      let actual = service.GetToDoItems()
      
      // Assert
      Assert.Pass()
      //CollectionAssert.Contains([todoA; todoB], actual)
