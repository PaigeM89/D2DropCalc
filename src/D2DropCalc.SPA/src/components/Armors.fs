namespace D2DropCalc.SPA

module Armors =
  open Feliz
  open Feliz.UseDeferred
  open Fulma
  open Fable.React
  open Fable.React.Props
  open Fable.FontAwesome

  open D2DropCalc.Types.Items

  let armorTableRow(armor : Armor) =
    let tostr (io : int option) =
          match io with
          | Some i -> i.ToString()
          | None -> "-"
    Html.tableRow [
        Html.tableCell [ Html.p armor.Name]
        Html.tableCell [ Html.p armor.Code]
        Html.tableCell [ Html.p (tostr armor.ItemLevel)]
        Html.tableCell [ Html.p (tostr armor.ReqLevel)]
        Html.tableCell [ Html.p (tostr armor.ReqStrength)]
        Html.tableCell [ Html.p (tostr armor.MaxSockets)]
        Html.tableCell [ Html.p (armor.BaseType.ToString()) ]
    ]

  let armorTableHeader() =
    Html.tableRow [
        Html.tableHeader [Html.h3 "Name"]
        Html.tableHeader [Html.h3 "Code"]
        Html.tableHeader [Html.h3 "Item Level"]
        Html.tableHeader [Html.h3 "Req Level"]
        Html.tableHeader [Html.h3 "Req Strength"]
        Html.tableHeader [Html.h3 "Max Sockets"]
        Html.tableHeader [ Html.h3 "Base Item Type" ]
    ]

  [<ReactComponent>]
  let LoadArmors() =
      let (armors, setArmors) = React.useState(Deferred.HasNotStartedYet)
      printfn "in load armors component"

      let loadData = Fetch.getArmors()

      let startLoading = React.useDeferredCallback((fun () -> loadData), setArmors)
      React.useEffect(startLoading, [||])
      match armors with
      | Deferred.HasNotStartedYet -> Html.h1 "Not Started"
      | Deferred.InProgress -> 
          div [ Class ("block " + Fa.Classes.Size.Fa3x) ] [
                Fa.i [ Fa.Solid.Spinner; Fa.Spin ] []
          ]
      | Deferred.Failed error -> Html.h1 error.Message
      | Deferred.Resolved (Ok content) ->
          let tableProps = [
              Table.IsBordered
              Table.IsStriped
          ]
          let head = armorTableHeader()
          let rows = content |> List.map armorTableRow
          Table.table [
              yield! tableProps
          ] [
              thead [] [ head]
              tbody [] rows
          ]
      | Deferred.Resolved ((Error fetchError)) ->
        Html.h1 $"Error fetching data: %A{fetchError}"