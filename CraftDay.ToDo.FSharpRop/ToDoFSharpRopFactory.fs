namespace CraftDay.ToDo.FSharpRop

open CraftDay.ToDo.Common.Services

type ToDoFSharpRopFactory() =
  inherit ToDoServiceFactory()

  let mutable maybe_store: IToDoStore option = None
  override this.WithStore(store) =
    maybe_store <- Some store
    this :> ToDoServiceFactory
    
  override this.Build() =
    match maybe_store with
    | Some store -> (ToDoFSharpRopService store) :> IToDoService
    | None -> "Store is None" |> exn |> raise
