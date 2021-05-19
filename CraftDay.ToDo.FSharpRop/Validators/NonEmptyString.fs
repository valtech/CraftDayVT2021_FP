namespace CraftDay.ToDo.FSharpRop.Validators

open CraftDay.ToDo.CSharp.Errors


type NonEmptyString = private NonEmptyString of string

module NonEmptyString =
  let create (str: string): Result<NonEmptyString, ValidationException> =
    if System.String.IsNullOrEmpty str
    then "String is not allowed to be null" |> ValidationException |> Error
    else NonEmptyString str |> Ok
  
  let value (NonEmptyString str): string =
    str