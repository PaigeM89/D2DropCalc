namespace D2DropCalc.SPA.Components

module ItemBaseDropdowns =
  open Feliz
  open Feliz.UseDeferred
  open Fulma
  open Fable.React
  open Fable.React.Props
  open Fable.FontAwesome
  open D2DropCalc.SPA
  open D2DropCalc.SPA.Components

  open D2DropCalc.Types.Items

  let armorDDLValue (armor : Armor) =
    Micro.Dropdown.createDDLValue armor.Code armor.Name

  let weaponDDLValue (weap : Weapon) =
    Micro.Dropdown.createDDLValue weap.Code weap.Name

  [<ReactComponent>]
  let ItemDropdowns() =
    let (armors, setArmors) = React.useState(Deferred.HasNotStartedYet)
    let (weapons, setWeapons) = React.useState(Deferred.HasNotStartedYet)

    let startLoadingArmors = React.useDeferredCallback((fun () -> Fetch.getArmors()), setArmors)
    let startLoadingWeapons = React.useDeferredCallback((fun () -> Fetch.getWeapons()), setWeapons)

    React.useEffect(startLoadingArmors, [||])
    React.useEffect(startLoadingWeapons, [||])

    match armors, weapons with
    | Deferred.Resolved (Ok armors), Deferred.Resolved (Ok weapons) ->
      let ddlValues = (armors |> List.map armorDDLValue) @ (weapons |> List.map weaponDDLValue)
      Micro.Dropdown.SearchableWithPlaceholder ddlValues "Select an item base"
    | Deferred.Resolved (Error e), Deferred.Resolved _->
      printfn "error loading armors: %A" e
      Micro.Error.ErrorMessage "Unable to load data"
    | Deferred.Resolved _, Deferred.Resolved (Error e) ->
      printfn "error loading weapons: %A" e
      Micro.Error.ErrorMessage "Unable to load data"
    | Deferred.Failed error, _ ->
      printfn "deffered error: %A" error
      Micro.Error.ErrorMessage "Unable to resolve data load"
    | _, Deferred.Failed error ->
      printfn "deffered error: %A" error
      Micro.Error.ErrorMessage "Unable to resolve data load"
    | _ ->
      div [ Class ("block " + Fa.Classes.Size.Fa3x) ] [
        Fa.i [ Fa.Solid.Spinner; Fa.Spin ] []
      ]

  // i think this causes react dom errors due to hook stuff
  let private renderSearch weapons armors =
    let (selectValue, setSelected) = React.useState(None)
    let (ddlValues, setDDLValues) = React.useState([])
    if ddlValues = [] then
      let values = (armors |> List.map armorDDLValue) @ (weapons |> List.map weaponDDLValue)
      setDDLValues values
    
    let onChange (value : string) =
      printfn "on change function fired, value is %s" value

    let props = {
      Micro.Dropdown.SearchableProps.elements = ddlValues
      Micro.Dropdown.SearchableProps.placeholder = "Select an item base"
      Micro.Dropdown.SearchableProps.callback = onChange
    }

    let searchable = Micro.Dropdown.SearchableWithFunc props
    div [] [
      Columns.columns [] [
        Column.column [ Column.Width(Screen.All, Column.Is2) ] [
          searchable
        ]
      ]
      match selectValue with
      | Some sv ->
        Label.label [] [str ($"You selected: '%A{sv}'") ]
      | None -> Html.none
    ]

  let ItemDropdownFunc() =
    let (armors, setArmors) = React.useState(Deferred.HasNotStartedYet)
    let (weapons, setWeapons) = React.useState(Deferred.HasNotStartedYet)

    let startLoadingArmors = React.useDeferredCallback((fun () -> Fetch.getArmors()), setArmors)
    let startLoadingWeapons = React.useDeferredCallback((fun () -> Fetch.getWeapons()), setWeapons)

    React.useEffect(startLoadingArmors, [||])
    React.useEffect(startLoadingWeapons, [||])

    match armors, weapons with
    | Deferred.Resolved (Ok armors), Deferred.Resolved (Ok weapons) ->
      //let ddlValues = (armors |> List.map armorDDLValue) @ (weapons |> List.map weaponDDLValue)
      //Micro.Dropdown.SearchableWithPlaceholder ddlValues "Select an item base"
      
      renderSearch weapons armors
    | Deferred.Resolved (Error e), Deferred.Resolved _->
      printfn "error loading armors: %A" e
      Micro.Error.ErrorMessage "Unable to load data"
    | Deferred.Resolved _, Deferred.Resolved (Error e) ->
      printfn "error loading weapons: %A" e
      Micro.Error.ErrorMessage "Unable to load data"
    | Deferred.Failed error, _ ->
      printfn "deffered error: %A" error
      Micro.Error.ErrorMessage "Unable to resolve data load"
    | _, Deferred.Failed error ->
      printfn "deffered error: %A" error
      Micro.Error.ErrorMessage "Unable to resolve data load"
    | _ ->
      div [ Class ("block " + Fa.Classes.Size.Fa3x) ] [
        Fa.i [ Fa.Solid.Spinner; Fa.Spin ] []
      ]
