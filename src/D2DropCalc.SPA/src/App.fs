module App

open Feliz
open Feliz.UseDeferred
open Fable.SimpleHttp
open Fable.React
open Fulma

open D2DropCalc.SPA.Components

[<ReactComponent>]
let RootComponent() =
    div [  ] [
        D2DropCalc.SPA.Components.Navbar.Navbar()
        Columns.columns [  ] [
            Column.column [] [
                Container.container [] [
                    ViewItem.SelectAndViewItem()
                    MonsterDrop.monsterSearchDropdown( { callback = fun _ -> () } )
                ]
            ]
        ]
    ]


open Browser.Dom

ReactDOM.render(RootComponent(), document.getElementById "root")

