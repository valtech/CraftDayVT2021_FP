namespace CraftDay.ToDo.FSharpRop

open System
open System.Net
open CraftDay.ToDo.CSharp.Errors
open CraftDay.ToDo.Common.Dto
open CraftDay.ToDo.Common.Services
open CraftDay.ToDo.FSharpRop.Validators
open Newtonsoft.Json

module ToDoFSharpRopWorkflow =
  type HttpResult = HttpStatusCode * string
  type UnvalidatedId = string
  type WorkflowError =
    | ValidationError of ValidationException
    | DomainError of DomainException
    | DaoError of DaoException
  
  type IToDoWorkflow = {
    getAllItems: unit -> HttpResult 
    getItem: string -> HttpResult 
  }
  
  type Services = {
    getAllItems: unit -> Result<ToDoGetItemsMessage, WorkflowError> 
    getItem: UnvalidatedId -> Result<ToDoGetItemsMessage, WorkflowError> 
  }
  
  let private wrapInEnvelope item =
    ToDoGetItemsMessage(items = ([item] |> ResizeArray))
  
  let private getItems (service: IToDoService) =
    try
      service.GetAllItems() |> Ok
    with
    | ex -> DomainException(ex.Message) |> Error
  
  let private doGetAllItems (service: IToDoService) _ =
    (getItems service) // Get values from store
    |> Result.mapError DomainError // Convert error to same as flow
    |> Result.bind (fun items -> ToDoGetItemsMessage(items = items) |> Ok) // Map to envelope type
  
  let private convertToId (str: NonEmptyString): Result<int, WorkflowError> =
    match Int32.TryParse (NonEmptyString.value str) with
    | true, v -> v |> Ok
    | false, _e ->
      "String is not a valid ID"
      |> ValidationException
      |> WorkflowError.ValidationError
      |> Error
  
  let private getItemFromService (service: IToDoService) id =
    try
      id
      |> service.GetItem
      |> Ok
    with
    | ex -> DaoException(ex.Message) |> WorkflowError.DaoError |> Error
  
  let private doGetItem (service: IToDoService) unvalidatedId: Result<ToDoGetItemsMessage, WorkflowError> =
    unvalidatedId
    |> NonEmptyString.create
    |> Result.mapError WorkflowError.ValidationError
    |> Result.bind convertToId
    |> Result.bind (getItemFromService service)
    |> Result.bind (wrapInEnvelope >> Ok)
  
  let private convertError (err: WorkflowError): HttpResult =
    match err with
    | ValidationError e -> HttpStatusCode.BadRequest, e.ToString()
    | DomainError e -> HttpStatusCode.BadRequest, e.ToString()
    | DaoError e ->
      HttpStatusCode.InternalServerError,
      {| ``type`` = "InternalError"; error = e.Message |}
      |> JsonConvert.SerializeObject
  
  let private toHttpResult (res: Result<ToDoGetItemsMessage, WorkflowError>): HttpResult =
    match res with
    | Ok msg -> (HttpStatusCode.OK, msg |> JsonConvert.SerializeObject)
    | Error e -> e |> convertError
  
  let setup (service: IToDoService): IToDoWorkflow = {
    getAllItems = (doGetAllItems service >> toHttpResult)
    getItem = (doGetItem service >> toHttpResult)
  }
  