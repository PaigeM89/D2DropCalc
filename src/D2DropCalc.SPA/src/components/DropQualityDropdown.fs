namespace D2DropCalc.SPA.Components


module DropQualityDropdown =
  open Fable.React
  open Fable.React.Props
  open Feliz
  open Feliz.UseDeferred
  open Fable.FontAwesome
  open D2DropCalc.SPA
  open D2DropCalc.Types.Monsters
  open Fulma
  open Micro.Dropdown

  type DropQualityDropdownProps = {
    callback : MonsterDropQuality -> unit
  }

  let DropQualityDropdown = React.functionComponent(fun (props: DropQualityDropdownProps) ->
    let options = [
      createDDLValue (string MonsterDropQuality.Normal) (string MonsterDropQuality.Normal)
      createDDLValue (string Champion) (string Champion)
      createDDLValue (string Unique) (string Unique)
      createDDLValue (string Quest) (string Quest)
    ]
    let callback (str : string) =
      match MonsterDropQuality.FromString str with
      | Ok x -> 
        printfn $"invoke monster quality callback with arg '%A{x}' here"
        props.callback x
      | Error e -> printfn $"error parsing monster quality from id: %s{e}"

    let props = {
      elements = options
      placeholder = "Select a monster quality"
      callback = callback
    }

    div [ Class "mon-qual-dropdown" ] [
      SearchableWithFunc props
    ]
  )