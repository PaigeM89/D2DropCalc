namespace D2DropCalc.SPA.Components


module MonsterDrop =
  open Fable.React
  open Fable.React.Props
  open Feliz
  open Feliz.UseDeferred
  open Fable.FontAwesome
  open D2DropCalc.SPA
  open D2DropCalc.Types
  open D2DropCalc.Types.Items
  open Fulma
  open Micro.Dropdown

  let viewMonsterDetails = React.functionComponent(fun (monster : Monsters.Monster) ->
    let treasureClassNames =
      monster.ItemTreeEntrypoints |> List.map (fun ep -> ep.ItemTreeNode) |> String.concat ", "
      |> fun str -> if str = "" then "None" else str
    let intStrOrNone (io : int option) =
      match io with
      | Some x -> string x
      | None -> "None"
    div [] [
      Label.label [] [ str monster.Name ]
      Label.label [] [ str ("Level: " + (string monster.Level)) ]
      if monster.LevelNightmare.IsSome then Label.label [] [ str ("Level (N): " + (string monster.LevelNightmare.Value))]
      if monster.LevelHell.IsSome then Label.label [] [ str ("Level (N): " + (string monster.LevelHell.Value))]
      Label.label [] [ str ("Treasure Classes: " + treasureClassNames) ]
    ]
  )

  type SearchDropdownProps = {
    callback : string -> unit
  }

  let monstersToDDL (monsters : Monsters.Monster list) =
    printfn "creating monster ddl list"
    monsters |> List.map (fun m -> Micro.Dropdown.createDDLValue m.Id m.Name)

  let monsterDDLProps callback values = {
    elements = values
    placeholder = "Select a monster"
    callback = callback
  }

  let monsterSearchDropdown = React.functionComponent(fun (props : SearchDropdownProps) ->
    printfn "in monsters search dropdown"
    let (monsters, setMonsters) = React.useState(Deferred.HasNotStartedYet)
    let (selectedMonster, setSelectedMonster) = React.useState(None)
    let (selected, setSelected) = React.useState(None)
    let (ddlValues, setDDLValues) = React.useState([])
    let startLoadingMonsters = React.useDeferredCallback((fun () -> Fetch.getMonsters()), setMonsters)
    React.useEffect(startLoadingMonsters, [||])

    let setMonster (monsters : Monsters.Monster list) (id : string) =
      printfn "trying to set selected monster for id %s" id
      match monsters |> List.tryFind (fun x -> x.Id = id) with
      | Some v -> setSelectedMonster (Some v)
      | None -> ()

    let selectionChanged id =
      printfn "in selection change callback being invoked, id is %s" id
      match selected with
      | Some x when x = id -> ()
      | _ ->
        setSelected (Some id)
        props.callback id

    match monsters with
    | Deferred.Resolved (Ok monsters) ->
      printfn "deferred resolved ok"
      let ddlValues =
        if List.isEmpty ddlValues then
          printfn "ddlvalues are empty, populating"
          let values = monstersToDDL monsters
          setDDLValues values
          printfn "returning list of ddl values"
          values
         else
          printfn "ddl values found already, returning"
          ddlValues
      
      printfn "about to render dropdown from caller"

      let onChange id =
        printfn "in onchange handler"
        setMonster monsters id
        selectionChanged id

      let ddl = 
        monsterDDLProps onChange ddlValues
        |> SearchableWithFunc 
      let deets = 
        match selectedMonster with
        | Some monster ->
          printfn "viewing monster details for monster %A" monster
          viewMonsterDetails monster
        | None -> Html.none

      div [] [
        ddl
        deets
      ]
    | Deferred.Resolved (Error e) ->
      printfn "in error handler for resolved deferred"
      Micro.Error.ErrorMessage "There was an error fetching or decoding monster data"
    | Deferred.Failed e ->
      printfn "in failed handler for deferred"
      Micro.Error.ErrorMessage $"There was an error using deferred data loading: %A{e}"
    | _ ->
      div [ Class ("block " + Fa.Classes.Size.Fa3x) ] [
          Fa.i [ Fa.Solid.Spinner; Fa.Spin ] []
      ]
  )