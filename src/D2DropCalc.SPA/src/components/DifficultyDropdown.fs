namespace D2DropCalc.SPA.Components


module DifficultyDropdown =
  open Fable.React
  open Fable.React.Props
  open Feliz
  open Feliz.UseDeferred
  open Fable.FontAwesome
  open D2DropCalc.SPA
  open D2DropCalc.Types.ItemTree
  open Fulma
  open Micro.Dropdown

  type DiffDropdownProps = {
    callback : Difficulty -> unit
  }

  let DifficultyDropdown = React.functionComponent(fun ( props : DiffDropdownProps ) ->
    let options = [
      createDDLValue (string Difficulty.Normal) (string Difficulty.Normal)
      createDDLValue (string Difficulty.Nightmare) (string Difficulty.Nightmare)
      createDDLValue (string Difficulty.Hell) (string Difficulty.Hell)
    ]
    let callback (str : string) =
      match Difficulty.FromString str with
      | Ok x ->
        printfn $"invoke callback with arg '%A{x}' here"
        props.callback x
      | Error e -> printfn $"error parsing difficulty from id: %s{e}"


    let props = {
      elements = options
      placeholder = "Select a difficulty"
      callback = callback
    }
    div [ Class "diff-dropdown" ] [
      SearchableWithFunc props
    ]
  )