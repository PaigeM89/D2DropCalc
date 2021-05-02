module App

open Feliz
open Feliz.UseDeferred
open Fable.SimpleHttp
open Fable.React
open Fulma

open D2DropCalc.SPA.Components
open D2DropCalc.SPA.Components.ViewItem
open D2DropCalc.SPA.Components.DifficultyDropdown
open D2DropCalc.SPA.Components.DropQualityDropdown
open D2DropCalc.SPA.Components.MonsterDrop

// [<ReactComponent>]
// let RootComponent() =
//     let (itemCode, setItemCode) = React.useState(None)
//     let itemCallback str =
//         printfn $"Root setting item code to '%s{str}"
//         setItemCode (Some str)

//     let (diff, setDiff) = React.useState(None)
//     let diffCallback d =
//         printfn $"Root setting difficulty to %A{d}"
//         setDiff (Some d)

//     let (qual, setQual) = React.useState(None)
//     let qualCallback q =
//         printfn $"Root setting monster quality to %A{q}"
//         setQual (Some q)

//     let (monster, setMonster) = React.useState(None)
//     let monsterCallback str =
//         printfn $"Root setting monster id to '%s{str}"
//         setMonster (Some str)

//     let (treasureClass, setTreasureClass) = React.useState(None)
//     let tcPickedCallback str =
//         printfn $"Root setting picked treasure class to '%s{str}"
//         setTreasureClass (Some str)

//     div [  ] [
//         D2DropCalc.SPA.Components.Navbar.Navbar()
//         Container.container [] [
//             Columns.columns [  ] [
//                 Column.column [] [
//                     Hero.hero [] [
//                         //ViewItem.SelectAndViewItem()
//                         ViewItem.SelectAndViewItemWithCallback ({ SelectItemProps.callback = itemCallback })
//                     ]
//                 ]
//                 Column.column [ Column.Width (Screen.All, Column.Is4) ] [
//                     Level.level [] [
//                         Hero.hero [] [
//                             DifficultyDropdown( { callback = diffCallback })
//                         ]
//                         Hero.hero [] [
//                             DropQualityDropdown( { callback = qualCallback })
//                         ]
//                     ]
//                 ]
//                 Column.column [] [
//                     Hero.hero [] [
//                         MonsterDrop.monsterSearchDropdown( { MonsterDrop.callback = monsterCallback; difficulty = diff; quality = qual; treasureClassPickedCallback = tcPickedCallback } )
//                     ]
//                 ]
//             ]
//             CalcDrop.DropCalc( itemCode, treasureClass) // { itemCode = itemCode; difficulty = diff; quality = qual; monsterId = monster })
//         ]
//     ]


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
            | Some ddo ->
                CalcDrop.DropCalc( ddo.itemCode, ddo.treasureClass)
            | None ->
                Html.none
        ]
    ]

open Browser.Dom

ReactDOM.render(RootComponent(), document.getElementById "root")

