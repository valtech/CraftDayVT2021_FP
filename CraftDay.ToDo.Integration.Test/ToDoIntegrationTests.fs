module CraftDay.ToDo.Integration.Test.ToDoTests

open CraftDay.ToDo.CSharp
open CraftDay.ToDo.CSharpRop
open CraftDay.ToDo.Common.Dto
open CraftDay.ToDo.Util.Test.Utils
open CraftDay.ToDo.Integration.Test
open NUnit.Framework
open Newtonsoft.Json

type ToDoApi = {
  getAllItems: unit -> HttpResult
  getItem: Param -> HttpResult
} 

let todoA = ToDoItem (TaskDescription = "TODO A")
let todoB = ToDoItem (TaskDescription = "TODO B")
let store = DummyToDoStore()
do store.AddAll([(1, todoA); (2, todoB)])
let service = ToDoCSharpService(store)
let controller = ToDoController(service)
let cSharpWorkflow = ToDoCSharpWorkflow(store)

[<TestFixture>]
type TodoIntegrationTests() =
  static member Apis: ToDoApi list = [
    ({
      getAllItems = controller.GetAllItems
      getItem = controller.GetItem
    }: ToDoApi) // C# API
    ({
      getAllItems = cSharpWorkflow.GetAllItems
      getItem = cSharpWorkflow.GetItem
    }: ToDoApi) // C# ROP API
  ]
  
  static member SetupApiRoutes api =
    Map.empty
        .Add((GET, "/todo/"), api.getAllItems |> ParamOrBody.UnParamFunc)
        .Add((GET, "/todo/{}"), api.getItem |> ParamOrBody.ParamFunc)
    |> Router
    
  [<TestCaseSource("Apis")>]
  member _.``GET /todo/ -> return all TODOs`` (api: ToDoApi) =
    // Arrange    
    let route = TodoIntegrationTests.SetupApiRoutes api
    
    // Act
    let actual = route.call (GET, "/todo/") ""
    let actual = JsonConvert.DeserializeObject<ToDoGetItemsMessage>(actual)
    
    // Assert
    Assert.AreEqual(2, actual.items.Count)

  [<TestCaseSource("Apis")>]
  member _.``GET /todo/2 -> return TODO with id 2`` (api: ToDoApi) =
    // Arrange   
    let route = TodoIntegrationTests.SetupApiRoutes api
    
    // Act
    let actual = route.call (GET, "/todo/{}") "2"
    let actual = JsonConvert.DeserializeObject<ToDoGetItemsMessage>(actual)
    
    // Assert
    Assert.AreEqual(1, actual.items.Count)
    Assert.AreEqual(todoB.TaskDescription, actual.items.Item(0).TaskDescription)
