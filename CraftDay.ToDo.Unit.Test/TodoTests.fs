module CraftDay.ToDo.Unit.Test

open System.Collections.Generic
open CraftDay.ToDo.Common.Dto
open CraftDay.ToDo.Common.Services
open CraftDay.ToDo.CSharp
open CraftDay.ToDo.CSharpRop
open CraftDay.ToDo.FSharpRop
open CraftDay.ToDo.Util.Test.Utils
open NUnit.Framework
    

[<TestFixture>]
type TodoTests() =
  
  static member Services: ToDoServiceFactory list = [
    ToDoCSharpFactory() 
    ToDoCSharpRopFactory()
    ToDoFSharpRopFactory()
  ]
  
  [<TestCaseSource("Services")>]
  member _.``Can get TODO list`` (serviceFactory: ToDoServiceFactory) =
    // Arrange
    let todoA = ToDoItem (TaskDescription = "TODO A")
    let todoB = ToDoItem (TaskDescription = "TODO B")
    let todoStore = DummyToDoStore()
    todoStore.AddAll([(1, todoA); (2, todoB)])
    let service = serviceFactory.WithStore(todoStore).Build()
    
    // Act
    let actual = service.GetAllItems()
    
    // Assert
    Assert.AreEqual(2, actual.Count)
    Assert.Contains(todoA, actual)
    Assert.Contains(todoB, actual)
    
  [<TestCaseSource("Services")>]
  member _.``Can get a specific TODO item`` (serviceFactory: ToDoServiceFactory) =
    // Arrange
    let todoA = ToDoItem (TaskDescription = "TODO A")
    let todoB = ToDoItem (TaskDescription = "TODO B")
    let todoStore = DummyToDoStore()
    todoStore.AddAll([(1, todoA); (2, todoB)]) 
    let service: IToDoService = serviceFactory.WithStore(todoStore).Build()
    
    // Act
    let actual = service.GetItem(2)
    
    // Assert
    Assert.AreEqual(todoB, actual)
    Assert.AreEqual(todoB.TaskDescription, actual.TaskDescription)
