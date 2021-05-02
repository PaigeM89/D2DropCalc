namespace D2DropCalc.SPA.Components.Micro

module Dropdown =
  open Feliz
  open Feliz.SelectSearch
  open D2DropCalc.SPA.Components.Micro.Error

  type DDLValue = { Value : string; Name : string }

  let createDDLValue value name =
      { value = value; name = name; disabled = false }


  [<ReactComponent>]
  let Searchable (elements : DDLValue list) =
    let (selectValue, setSelected) = React.useState(None)
    let getValue (sv : string option) =
      match sv with
      | Some x -> selectSearch.value x
      | None -> selectSearch.placeholder "Select an option"
    SelectSearch.selectSearch [
      (getValue selectValue)
      selectSearch.search true
      selectSearch.onChange (fun (value : string) -> setSelected (Some value))
      selectSearch.options [
        for ele in elements do
          yield { value = ele.Value; name = ele.Name; disabled = false }
      ]
    ]

  [<ReactComponent>]
  let SearchableWithPlaceholder (elements : SelectItem list) ph =
    let (selectValue, setSelected) = React.useState(None)
    let getValue (sv : string option) =
      match sv with
      | Some x -> selectSearch.value x
      | None -> selectSearch.placeholder ph
    SelectSearch.selectSearch [
      (getValue selectValue)
      selectSearch.search true
      selectSearch.onChange (fun (value : string) -> setSelected (Some value))
      selectSearch.options elements
    ]

  type SearchableProps = {
    elements : SelectItem list
    placeholder : string
    callback : string -> unit
  }

  let SearchableWithFunc =
    React.functionComponent (fun (props: SearchableProps) ->
      let (selectValue, setSelected) = React.useState(None)
      let getValue (sv : string option) =
        match sv with
        | Some x -> selectSearch.value x
        | None -> selectSearch.placeholder props.placeholder
      SelectSearch.selectSearch [
        (getValue selectValue)
        selectSearch.search true
        selectSearch.onChange (fun (value : string) -> props.callback value; setSelected (Some value))
        selectSearch.options props.elements
      ]
    )


  type GroupedProps = {
    groupedElements : SelectOption list
    placeholder : string
    callback : string -> unit
  }

  let GroupedSearchable = React.functionComponent( fun (props : GroupedProps) ->
    let (selectValue, setSelected) = React.useState(None)
    let getValue (sv : string option) =
      match sv with
      | Some x -> selectSearch.value x
      | None -> selectSearch.placeholder props.placeholder
    SelectSearch.selectSearch [
      (getValue selectValue)
      selectSearch.search true
      selectSearch.onChange (fun (value : string) -> props.callback value; setSelected (Some value))
      selectSearch.options props.groupedElements
    ]
  )

  open Feliz.UseDeferred
  open Fable.React
  open Fable.React.Props
  open Fable.FontAwesome

  /// Loads & maps in a single step given a delegate.
  /// Must be able to extract required values from a select option in the unmapper
  [<ReactComponent>]
  let DropdownGrouped (callback: string -> unit, loaderAndMap: unit -> Async<Result<SelectOption list, string>>, placeholder) =
    let (selected, setSelected) = React.useState(None)
    let (loaderValues, setLoaderValues) = React.useState(Deferred.HasNotStartedYet)

    let invokeLoader() =
      printfn "Invoking loader in dropdown component"
      loaderAndMap()

    let startLoading = React.useDeferredCallback((fun () -> invokeLoader()), setLoaderValues)
    React.useEffect(startLoading, [||])

    match loaderValues with
    | Deferred.Resolved (Ok ddlValues) ->
      let selectionChangedCallback (eleStr : string) =
        printfn $"Selection in dropdown changed to '%A{eleStr}, invoking callback"
        callback eleStr

      let getValue (sv : string option) =
        match sv with
        | Some x -> selectSearch.value x
        | None -> selectSearch.placeholder placeholder
      SelectSearch.selectSearch [
        (getValue selected)
        selectSearch.search true
        selectSearch.onChange (fun (value : string) -> selectionChangedCallback value; setSelected (Some value))
        selectSearch.options ddlValues
      ]
    | Deferred.Resolved (Error e) -> ErrorMessage e
    | Deferred.Failed error ->
      printfn $"Deferred failed in dropdown: %A{error}"
      ErrorMessage "Error loading dropdown values"
    | _ ->
      div [ Class ("block " + Fa.Classes.Size.Fa3x) ] [
        Fa.i [ Fa.Solid.Spinner; Fa.Spin ] []
      ]

  [<ReactComponent>]
  let Dropdown (callback: string -> unit, loader: unit -> Async<Result<'a list, string>>, mapper: 'a -> SelectItem, placeholder) =
    let (selected, setSelected) = React.useState(None)
    let (loaderValues, setLoaderValues) = React.useState(Deferred.HasNotStartedYet)

    let invokeLoader() =
      printfn "Invoking loader in dropdown component"
      loader()

    let startLoading = React.useDeferredCallback((fun () -> invokeLoader()), setLoaderValues)
    React.useEffect(startLoading, [||])

    match loaderValues with
    | Deferred.Resolved (Ok values) ->
      let ddlValues : SelectItem seq = List.map mapper values |> Seq.ofList

      let selectionChangedCallback (eleStr : string) =
        printfn $"Selection in dropdown changed to '%A{eleStr}, invoking callback"
        callback eleStr

      let getValue (sv : string option) =
        match sv with
        | Some x -> selectSearch.value x
        | None -> selectSearch.placeholder placeholder
      SelectSearch.selectSearch [
        (getValue selected)
        selectSearch.search true
        selectSearch.onChange (fun (value : string) -> selectionChangedCallback value; setSelected (Some value))
        selectSearch.options ddlValues
      ]
    | Deferred.Resolved (Error error) ->
      ErrorMessage error
    | Deferred.Failed error ->
      printfn $"Deferred failed in dropdown: %A{error}"
      ErrorMessage "Error loading dropdown values"
    | _ ->
      div [ Class ("block " + Fa.Classes.Size.Fa3x) ] [
        Fa.i [ Fa.Solid.Spinner; Fa.Spin ] []
      ]


  [<ReactComponent>]
  let DropdownWithMatcher (callback: 'a -> unit, loader: unit -> Async<Result<'a list, string>>, mapper: 'a -> SelectItem, matcher: string -> 'a -> bool, placeholder) =
    let (selected, setSelected) = React.useState(None)
    let (loaderValues, setLoaderValues) = React.useState(Deferred.HasNotStartedYet)

    let invokeLoader() =
      printfn "Invoking loader in dropdown component"
      loader()

    let startLoading = React.useDeferredCallback((fun () -> invokeLoader()), setLoaderValues)
    React.useEffect(startLoading, [||])

    match loaderValues with
    | Deferred.Resolved (Ok values) ->
      let ddlValues : SelectItem seq = List.map mapper values |> Seq.ofList

      let selectionChangedCallback (eleStr : string) =
        printfn $"Selection in dropdown changed to '%A{eleStr}, invoking callback"
        let element = List.find (matcher eleStr) values
        callback element

      let getValue (sv : string option) =
        match sv with
        | Some x -> selectSearch.value x
        | None -> selectSearch.placeholder placeholder
      SelectSearch.selectSearch [
        (getValue selected)
        selectSearch.search true
        selectSearch.onChange (fun (value : string) -> selectionChangedCallback value; setSelected (Some value))
        selectSearch.options ddlValues
      ]
    | Deferred.Resolved (Error error) -> ErrorMessage error
    | Deferred.Failed error ->
      printfn $"Deferred failed in dropdown: %A{error}"
      ErrorMessage "Error loading dropdown values"
    | _ ->
      div [ Class ("block " + Fa.Classes.Size.Fa3x) ] [
        Fa.i [ Fa.Solid.Spinner; Fa.Spin ] []
      ]