module App

open Feliz
open Fable.React
open Fulma

open D2DropCalc.SPA.Components


[<ReactComponent>]
let RootComponent() =
    let (history, setHistory) = React.useState([])
    let addHistory (monster : string) (item : string) (chance : float) =
        match history with
        | [] ->
            let itm = (monster, item, chance)
            setHistory [itm]
        | (m, i, c) :: _ ->
            printfn "Comparing (%A,%A,%A) to (%A, %A, %A)" m i c monster item chance
            if m = monster && i = item && c = chance then
                ()
            else
                let itm = (monster,item,chance)
                setHistory (itm :: history)


    let (dropdownOutputs, setDDO) = React.useState(None)
    let ddoCallback (ddo : DropCalcDropdowns.DropdownOutputs) =
        setDDO (Some ddo)

    let calcChangeCallback (x : CalcDrop.CalcDropOutputs option) =
        printfn $"drop calc outputs: %A{x}"
        match dropdownOutputs, x with
        | Some { monster = Some monster; itemCode = Some item}, Some c ->
            addHistory monster.Name item c.chance
        | _ -> ()


    div [ ] [
        D2DropCalc.SPA.Components.Navbar.Navbar()
        Container.container [ Container.CustomClass "input-dropdowns" ] [
            DropCalcDropdowns.AllDropdowns (ddoCallback)
            match dropdownOutputs with
            /// only display the calc button when an item or monster has been selected
            /// in the future, only one of these will be needed to run a calculation.
            | Some ddo when ddo.itemCode.IsSome || ddo.monster.IsSome ->
                CalcDrop.CalculateDropButton( ddo.itemCode, ddo.treasureClass, calcChangeCallback )
            | _ ->
                Html.none

            CalcDisplay.DisplayCalculations(history)
        ]
    ]

open Browser.Dom

ReactDOM.render(RootComponent(), document.getElementById "root")

