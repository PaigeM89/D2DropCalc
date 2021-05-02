namespace D2DropCalc.SPA.Components

module CalcDisplay =
  open Feliz
  open Fable.React
  open Fable.React.Props
  open Fulma

  [<ReactComponent>]
  let DisplayCalculationRow(monsterName : string, itemName : string, chance : float) =
    li [] [
      Level.level [] [
        h3 [] [ str monsterName ]
        h3 [] [ str itemName ]
        h3 [] [ str (string (chance * 100.) + "%") ]
      ]
    ]

  [<ReactComponent>]
  let DisplayCalculationHistory(monsterName : string, itemName : string, chance : float) = 
    let (existingRows, setRows) = React.useState([])
    let newRow = DisplayCalculationRow(monsterName, itemName, chance)
    let allRows = newRow :: existingRows
    ul [] allRows
