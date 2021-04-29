namespace D2DropCalc.SPA.Components.Micro

module Error =
  open Feliz
  open Fable.React
  open Fulma

  [<ReactComponent>]
  let ErrorMessage( msg : string) = 
    Message.message [ Message.Color IsDanger ] [
      Message.header [] [ str "Error" ]
      Message.body [] [ str msg ]
    ]