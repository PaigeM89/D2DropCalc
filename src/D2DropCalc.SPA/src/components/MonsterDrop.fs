namespace D2DropCalc.SPA.Components

open D2DropCalc.Types.ItemTree


module MonsterDrop =
  open Fable.React
  open Fable.React.Props
  open Feliz
  open Feliz.UseDeferred
  open Fable.FontAwesome
  open D2DropCalc.SPA
  open D2DropCalc.Types
  open D2DropCalc.Types.Monsters
  open Fulma
  open Micro.Dropdown

  type MonsterDetailsProps = {
    monster : Monster
    difficulty : ItemTree.Difficulty option
    quality : MonsterDropQuality option
  }

  /// Attempts to find a treasure class for a monster given all inputs
  let getTreasureClass (monster : Monster) (diff : Difficulty option) (qual : MonsterDropQuality option) =
      match diff, qual with
      | Some d, Some q ->
          monster.ItemTreeEntrypoints |> List.tryFind( fun ep -> ep.Diff = d && ep.Quality = q)
      | _ -> None

  /// Returns TRUE if all 3 options have been selected and - hypothetically - we've limited to a single treasure class
  /// This doesn't check the monster actually HAS that treasure class as an option
  /// for example, 'quest' selection won't always turn up a treasure class.
  let treasureClassPicked monster diff qual =
      match monster, diff, qual with
      | Some m, Some d, Some q -> true
      | _ -> false

  [<ReactComponent>]
  let ViewMonsterDetails (monster, diff, qual) = // (props : MonsterDetailsProps) =
    // let monster = props.monster
    // let diff = props.difficulty
    // let qual = props.quality

    let inline optionMatch (x: 'a) (xo : 'a option) =
      match xo with
      | Some x' -> x = x'
      | None -> false

    let buildLi (ep : Entrypoint) =
      let css =
        if treasureClassPicked (Some monster) diff qual then
          if optionMatch ep.Diff diff && optionMatch ep.Quality qual then
            "exact-tc-match"
          else "no-tc-match"
        else
          if optionMatch ep.Diff diff || optionMatch ep.Quality qual then
            "partial-tc-match"
          else
            "no-tc-match"

      li [ ClassName css ] [ str ep.ItemTreeNode ]

    let tcs = monster.ItemTreeEntrypoints |> List.map buildLi
    let intStrOrNone (io : int option) =
      match io with
      | Some x -> string x
      | None -> "None"
    div [] [
      Label.label [] [ str ("Level: " + (string monster.Level)) ]
      if monster.LevelNightmare.IsSome then Label.label [] [ str ("Level (N): " + (string monster.LevelNightmare.Value))]
      if monster.LevelHell.IsSome then Label.label [] [ str ("Level (H): " + (string monster.LevelHell.Value))]
      Label.label [] [ str ("Treasure Classes: ") ]
      ul [] tcs
    ]


  type SearchDropdownProps = {
    callback : string -> unit
    difficulty : ItemTree.Difficulty option
    quality : MonsterDropQuality option
    treasureClassPickedCallback : string -> unit
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
      | Some v ->
          match getTreasureClass v props.difficulty props.quality with
          | Some tc -> props.treasureClassPickedCallback tc.ItemTreeNode
          | None -> ()
          setSelectedMonster (Some v)
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
          ViewMonsterDetails (monster, props.difficulty, props.quality)
            //({ monster = monster; difficulty = props.difficulty; quality = props.quality})
        | None -> Html.none

      div [ Class "monster-dropdown"] [
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
