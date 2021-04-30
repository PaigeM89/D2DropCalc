namespace D2DropCalc.SPA.Components

module ViewItem =
  open Fable.React
  open Fable.React.Props
  open Feliz
  open Feliz.UseDeferred
  open Fable.FontAwesome
  open D2DropCalc.SPA
  open D2DropCalc.Types.Items
  open D2DropCalc.Types.ItemTree
  open Fulma

  module private ItemTemplates =
    let intStrOrNone (io : int option) =
      match io with
      | Some x -> string x
      | None -> "None"

    [<ReactComponent>]
    let ViewArmor (armor : Armor) =
      div [] [
        Label.label [] [ str "Armor stuff goes here" ]
      ]

    let ViewWeapon (weapon : Weapon) =
      div [] [
        Label.label [] [ str weapon.Name ]
        Label.label [] [ str ("Type: " + weapon.Type)]
        Label.label [] [ str ("Base Quality: " + (weapon.BaseType.ToString()))]
        Label.label [] [ str ("Item Level: " + (intStrOrNone weapon.ItemLevel)) ]
        Label.label [] [ str ("Max Sockets: " + (intStrOrNone weapon.MaxSockets)) ]
        Label.label [] [ str ("Req Level: " + (intStrOrNone weapon.ReqLevel)) ]
        Label.label [] [ str ("Required Strength: " + (intStrOrNone weapon.ReqStrength)) ]
        Label.label [] [ str ("Required Dexterity: " + (intStrOrNone weapon.ReqDex)) ]
      ]

  type SearchProps = {
    weapons : Weapon list
    armors : Armor list
    selectedCode : string option
    selectionChangeCallback : ItemCode -> unit
  }

  let private lookupCodeAndRender = React.functionComponent(fun (props : SearchProps) ->
    let (armorMap, setArmorMap) = React.useState(Map.empty)
    let (weaponMap, setWeaponMap) = React.useState(Map.empty)

    let armorMap = if Map.isEmpty armorMap then props.armors |> List.map (fun x -> x.Code,x) |> Map.ofList else armorMap
    let weaponMap = if Map.isEmpty weaponMap then props.weapons |> List.map (fun x -> x.Code,x) |> Map.ofList else weaponMap

    let tryArmors c = Map.tryFind c armorMap
    let tryWeapons c = Map.tryFind c weaponMap

    match props.selectedCode with
    | Some code ->
      match tryArmors code with
      | Some armor -> ItemTemplates.ViewArmor armor
      | None -> Label.label [] [str "You did not select an armor"]
    | None ->
      Label.label [] [ str "No item selected" ]
  )

  let armorDDLValue (armor : Armor) =
    Micro.Dropdown.createDDLValue armor.Code armor.Name

  let weaponDDLValue (weap : Weapon) =
    Micro.Dropdown.createDDLValue weap.Code weap.Name

  let private renderSearch = React.functionComponent(fun (props : SearchProps) ->
    let (selectValue, setSelected) = React.useState(None)
    let (ddlValues, setDDLValues) = React.useState([])
    if ddlValues = [] then
      let values = (props.armors |> List.map armorDDLValue) @ (props.weapons |> List.map weaponDDLValue)
      setDDLValues values
    
    let onChange (value : string) =
      printfn "on change function fired, value is %s" value
      setSelected (Some value)

    let ddlProps = {
      Micro.Dropdown.SearchableProps.elements = ddlValues
      Micro.Dropdown.SearchableProps.placeholder = "Select an item base"
      Micro.Dropdown.SearchableProps.callback = onChange
    }

    let searchable = Micro.Dropdown.SearchableWithFunc ddlProps
    div [] [
      Columns.columns [] [
        Column.column [ Column.Width(Screen.All, Column.Is2) ] [
          searchable
        ]
      ]
      match selectValue with
      | Some sv ->
        //Label.label [] [str ($"You selected: '%A{sv}'") ]
        lookupCodeAndRender { props with selectedCode = (Some sv)}
      | None -> Html.none
    ]
  )

  [<ReactComponent>]
  let SelectAndViewItem() =
    let (armors, setArmors) = React.useState(Deferred.HasNotStartedYet)
    let (weapons, setWeapons) = React.useState(Deferred.HasNotStartedYet)

    let startLoadingArmors = React.useDeferredCallback((fun () -> Fetch.getArmors()), setArmors)
    let startLoadingWeapons = React.useDeferredCallback((fun () -> Fetch.getWeapons()), setWeapons)

    React.useEffect(startLoadingArmors, [||])
    React.useEffect(startLoadingWeapons, [||])

    match armors, weapons with
    | Deferred.Resolved (Ok armors), Deferred.Resolved (Ok weapons) ->
      let props = { weapons = weapons; armors = armors; selectedCode = None; selectionChangeCallback = fun _ -> ()}
      renderSearch props
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

  type SelectItemProps = { callback : ItemCode -> unit }

  let SelectAndViewItemWithCallback = React.functionComponent(fun (props : SelectItemProps) -> 
    let (armors, setArmors) = React.useState(Deferred.HasNotStartedYet)
    let (weapons, setWeapons) = React.useState(Deferred.HasNotStartedYet)

    let startLoadingArmors = React.useDeferredCallback((fun () -> Fetch.getArmors()), setArmors)
    let startLoadingWeapons = React.useDeferredCallback((fun () -> Fetch.getWeapons()), setWeapons)

    React.useEffect(startLoadingArmors, [||])
    React.useEffect(startLoadingWeapons, [||])

    match armors, weapons with
    | Deferred.Resolved (Ok armors), Deferred.Resolved (Ok weapons) ->
      let props = { weapons = weapons; armors = armors; selectedCode = None; selectionChangeCallback = props.callback}
      renderSearch props
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
  
  )