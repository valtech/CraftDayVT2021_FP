module CraftDay.ToDo.Integration.Test.ToDoTests

open System.Net
open CraftDay.ToDo.CSharp
open CraftDay.ToDo.CSharpRop
open CraftDay.ToDo.Common.Dto
open CraftDay.ToDo.FSharpRop
open CraftDay.ToDo.Util.Test.Utils
open CraftDay.ToDo.Integration.Test
open NUnit.Framework
open Newtonsoft.Json

type ToDoApi = {
  getAllItems: unit -> HttpResult
  getItem: Param -> HttpResult
  setIsDone: Param -> Body -> HttpResult
} 

let todoA = ToDoItem (TaskDescription = "TODO A")
let todoB = ToDoItem (TaskDescription = "TODO B")
let store = DummyToDoStore()
do store.AddAll([(1, todoA); (2, todoB)])
let service = ToDoCSharpService(store)
let controller = ToDoController(service)
let csharpWorkflow = ToDoCSharpWorkflow(store)
let fsharpWorkflow = ToDoFSharpRopWorkflow.setup (ToDoFSharpRopService store)

[<TestFixture>]
type TodoIntegrationTests() =
  static member Apis: ToDoApi list = [
    ({
      getAllItems = controller.GetAllItems
      getItem = controller.GetItem
      setIsDone = fun param body -> controller.SetIsDone(param, body)
    }: ToDoApi) // C# API
    ({
      getAllItems = csharpWorkflow.GetAllItems
      getItem = csharpWorkflow.GetItem
      setIsDone = fun param body -> csharpWorkflow.SetIsDone(param, body)
    }: ToDoApi) // C# ROP API
    ({
      getAllItems = fsharpWorkflow.getAllItems
      getItem = fsharpWorkflow.getItem
      setIsDone = fsharpWorkflow.setIsDone
    }: ToDoApi) // F# ROP API
  ]
  
  static member SetupApiRoutes api =
    Map.empty
        .Add((GET, "/todo/"), api.getAllItems |> ParamOrBody.UnParamFunc)
        .Add((GET, "/todo/{}"), api.getItem |> ParamOrBody.ParamFunc)
        .Add((GET, "/todo/{} {}"), api.setIsDone |> ParamOrBody.ParamAndBodyFunc)
    |> Router
    
  [<TestCaseSource("Apis")>]
  member _.``GET /todo/ -> return all TODOs`` (api: ToDoApi) =
    // Arrange    
    let route = TodoIntegrationTests.SetupApiRoutes api
    
    // Act
    let code, actual = route.call (GET, "/todo/") "" ""
    let actual = JsonConvert.DeserializeObject<ToDoGetItemsMessage>(actual)
    
    // Assert
    Assert.AreEqual(HttpStatusCode.OK, code)
    Assert.AreEqual(2, actual.items.Count)

  [<TestCaseSource("Apis")>]
  member _.``GET /todo/2 -> return TODO with id 2`` (api: ToDoApi) =
    // Arrange   
    let route = TodoIntegrationTests.SetupApiRoutes api
    
    // Act
    let code, actual = route.call (GET, "/todo/{}") "2" ""
    let actual = JsonConvert.DeserializeObject<ToDoGetItemsMessage>(actual)
    
    // Assert
    Assert.AreEqual(HttpStatusCode.OK, code)
    Assert.AreEqual(1, actual.items.Count)
    Assert.AreEqual(todoB.TaskDescription, actual.items.Item(0).TaskDescription)

  [<TestCaseSource("Apis")>]
  member _.``PUT /todo/2 -> set status to done and return TODO with id 2`` (api: ToDoApi) =
    // Arrange   
    let route = TodoIntegrationTests.SetupApiRoutes api
    let body = {| isDone = true |} |> JsonConvert.SerializeObject
    
    // Act
    let code, actual = route.call (GET, "/todo/{} {}") "2" body 
    let actual = JsonConvert.DeserializeObject<ToDoGetItemsMessage>(actual)
    
    // Assert
    Assert.AreEqual(HttpStatusCode.OK, code)
    Assert.AreEqual(1, actual.items.Count)
    Assert.AreEqual(todoB.TaskDescription, actual.items.Item(0).TaskDescription)
    Assert.AreEqual(true, actual.items.Item(0).IsDone)