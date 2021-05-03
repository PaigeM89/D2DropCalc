namespace D2DropCalc.SPA.Components

module CalcDisplay =
  open Feliz
  open Fable.React
  open Fable.React.Props
  open Fulma

  [<ReactComponent>]
  let DisplayCalculationRow(monsterName : string, itemName : string, chance : float) =
    tr [] [
      //Level.level [] [
        th [] [ str monsterName ]
        th [] [ str itemName ]
        th [] [ str (string (chance * 100.) + "%") ]
      //]
    ]

  let tryFindMatchingRow (element : ReactElement) (rows : ReactElement list) =
    printfn "looking for element %A" element
    List.tryFind (fun e -> e = element) rows

  [<ReactComponent>]
  let DisplayCalculations(calculations : (string * string * float) list) =
    printfn "Rendering %i rows" (List.length calculations)
    let rows = calculations |> List.map (fun (m, i, c) -> DisplayCalculationRow(m, i, c))
    
    if List.length rows > 0 then
      Table.table [
        Table.IsNarrow
        Table.IsBordered
        Table.IsStriped
        Table.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered)  ]
      ] [
        thead [] [
          tr [] [
            th [] [ h3 [] [ str "Monster Name" ]]
            th [] [ h3 [] [ str "Item Name" ]]
            th [] [ h3 [] [ str "% Chance" ]]
          ]
        ]
        tbody [] rows
      ]
    else
      Html.none
