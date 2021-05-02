module App

open Feliz
open Fable.React
open Fulma

open D2DropCalc.SPA.Components


[<ReactComponent>]
let RootComponent() =
    let (dropdownOutputs, setDDO) = React.useState(None)
    let ddoCallback ddo =
        setDDO (Some ddo)

    let (chance, setChance) = React.useState(None)
    let calcChangeCallback (x : CalcDrop.CalcDropOutputs) =
        printfn $"drop calc outputs: %A{x}"
        setChance (Some x)

    div [  ] [
        D2DropCalc.SPA.Components.Navbar.Navbar()
        Container.container [ Container.CustomClass "input-dropdowns" ] [
            DropCalcDropdowns.AllDropdowns (ddoCallback)
            match dropdownOutputs with
            /// only display the calc button when an item or monster has been selected
            /// in the future, only one of these will be needed to run a calculation.
            | Some ddo when ddo.itemCode.IsSome || ddo.monster.IsSome ->
                CalcDrop.DropCalc( ddo.itemCode, ddo.treasureClass, calcChangeCallback )
            | _ ->
                Html.none

            match chance, dropdownOutputs with
            | Some chance, Some { monster = Some monster} ->
                printfn "rendering calc display"
                CalcDisplay.DisplayCalculationHistory(monster.Name, chance.itemCode, chance.chance)
            | _ -> Html.none

        ]
    ]

open Browser.Dom

ReactDOM.render(RootComponent(), document.getElementById "root")

