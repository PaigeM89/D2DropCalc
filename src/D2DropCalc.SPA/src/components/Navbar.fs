namespace D2DropCalc.SPA.Components

module Navbar =
  open Feliz
  open Fable.React
  open Fable.React.Props
  open Fulma
  open Fable.FontAwesome

  module private SubComponents =

    let navButton href icon txt =
      Control.div [ ] [
        Button.a [ Button.Props [ Href href ] ] [
          Icon.icon [] [
            Fa.i [ icon ] []
          ]
          span [] [ str txt ]
        ]
      ]

  [<ReactComponent>]
  let Navbar() = 
    Navbar.navbar [ 
      Navbar.Color (IsCustomColor "diablo-red")
    ] [
      Container.container [] [
        Navbar.Start.div [] [
          Heading.h3 [ Heading.CustomClass "site-title" ] [ str "Diablo 2 Drop Calculator"]
        ]
        Navbar.Item.div [] [
          SubComponents.navButton "https://github.com/PaigeM89/D2DropCalc" Fa.Brand.Github "Github"
        ]
      ]
    ]