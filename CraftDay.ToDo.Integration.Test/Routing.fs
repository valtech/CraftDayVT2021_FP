namespace CraftDay.ToDo.Integration.Test

open System.Net

type Action = GET | POST | PUT
type Url = string
type Param = string
type Body = string
type HttpResult = (HttpStatusCode * Body)

type ParamOrBody =
  | UnParamFunc of (unit -> HttpResult)
  | ParamFunc of (Param -> HttpResult)
  | ParamAndBodyFunc of (Param -> Body -> HttpResult)

type ActionAndRoute = Action * Url
type RouteMapping = Map<ActionAndRoute, ParamOrBody>

type Router(mapping: RouteMapping) =
  member _.call (actionAndRoute: ActionAndRoute) (pathParam: string) (body: string) =
    let f = mapping.Item(actionAndRoute)
    match f with
    | UnParamFunc f -> f ()
    | ParamFunc f -> f pathParam
    | ParamAndBodyFunc f -> f pathParam body 