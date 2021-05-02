module App

open Feliz
open Feliz.UseDeferred
open Fable.SimpleHttp
open Fable.React
open Fulma

open D2DropCalc.SPA.Components


[<ReactComponent>]
let RootComponent() =
    let (dropdownOutputs, setDDO) = React.useState(None)
    let ddoCallback ddo =
        setDDO (Some ddo)

    div [  ] [
        D2DropCalc.SPA.Components.Navbar.Navbar()
        Container.container [ Container.CustomClass "input-dropdowns" ] [
            DropCalcDropdowns.AllDropdowns (ddoCallback)
            match dropdownOutputs with
            /// only display the calc button when an item or monster has been selected
            /// in the future, only one of these will be needed to run a calculation.
            | Some ddo when ddo.itemCode.IsSome || ddo.monster.IsSome ->
                CalcDrop.DropCalc( ddo.itemCode, ddo.treasureClass )
            | _ ->
                Html.none
        ]
    ]

open Browser.Dom

ReactDOM.render(RootComponent(), document.getElementById "root")

