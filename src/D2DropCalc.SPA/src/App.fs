module App

open Feliz
open Feliz.UseDeferred
open Fable.SimpleHttp
open Fable.React
open Fulma


[<ReactComponent>]
let RootComponent() =
    div [] [
        D2DropCalc.SPA.Components.Navbar.Navbar()
        Columns.columns [] [
            Column.column [] [
                Container.container [] [
                    D2DropCalc.SPA.Components.ViewItem.SelectAndViewItem()
                ]
            ]
        ]
    ]


open Browser.Dom

ReactDOM.render(RootComponent(), document.getElementById "root")

