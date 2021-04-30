namespace D2DropCalc.SPA.Components.Micro

module Dropdown =
  open Feliz
  open Feliz.SelectSearch

  type DDLValue = { Value : string; Name : string }

  let createDDLValue value name =
    { Value = value; Name = name}


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
  let SearchableWithPlaceholder (elements : DDLValue list) ph =
    let (selectValue, setSelected) = React.useState(None)
    let getValue (sv : string option) = 
      match sv with
      | Some x -> selectSearch.value x
      | None -> selectSearch.placeholder ph
    SelectSearch.selectSearch [
      (getValue selectValue)
      selectSearch.search true
      selectSearch.onChange (fun (value : string) -> setSelected (Some value))
      selectSearch.options [
        for ele in elements do
          yield { value = ele.Value; name = ele.Name; disabled = false }
      ]
    ]

  type SearchableProps = {
    elements : DDLValue list
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
        selectSearch.options [
          for ele in props.elements do
            yield { value = ele.Value; name = ele.Name; disabled = false }
        ]
      ]
    )