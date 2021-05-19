namespace CraftDay.ToDo.Integration.Test

type Action = GET | POST | PUT
type Url = string
type Param = string
type Body = string
type HttpResult = string

type ParamOrBody =
  | UnParamFunc of (unit -> HttpResult)
  | ParamFunc of (Param -> HttpResult)

type ActionAndRoute = Action * Url
type RouteMapping = Map<ActionAndRoute, ParamOrBody>

type Router(mapping: RouteMapping) =
  member _.call (actionAndRoute: ActionAndRoute) (param: string) =
    let f = mapping.Item(actionAndRoute)
    match f with
    | UnParamFunc f -> f ()
    | ParamFunc f -> f param