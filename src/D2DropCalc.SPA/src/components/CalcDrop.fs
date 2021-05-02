namespace D2DropCalc.SPA.Components


module CalcDrop =
  open Fable.React
  open Fable.React.Props
  open Feliz
  open Feliz.UseDeferred
  open Fable.FontAwesome
  open D2DropCalc.SPA
  open D2DropCalc.Types
  open Fulma
  open Micro.Dropdown

  type CalcDropProps = {
    itemCode : string option
    difficulty : ItemTree.Difficulty option
    quality : Monsters.MonsterDropQuality option
    monsterId : string option
  }

  let convertPropsToInput (itemCode : string option) (treasureClass) =
    match itemCode, treasureClass with
    | Some x, Some y -> DropCalculation.ItemAndNode (x, y) |> Some
    | _ -> None
    //DropCalculation.ItemAndNode (itemCode, treasureClass)

  //let DropCalc = React.functionComponent(fun (props : CalcDropProps) ->
  [<ReactComponent>]
  let DropCalc (itemCodeOpt, treasureClassOpt) =
    let inputs = convertPropsToInput itemCodeOpt treasureClassOpt
    let (dropChances, setDropChances) = React.useState(None)
        //React.useState(Map.empty)
    let (dropInputs, setDropInputs) = React.useState(inputs)

    let run() =
      async {
        match dropInputs with
        | Some inputs ->
          let! res = inputs |> Post.sendDropCalculation
          match res with
          | Ok f -> setDropChances (Some(f))
          | Error e -> setDropChances None
        | None -> setDropChances None
      }
      |> Async.StartImmediate

    React.useEffect(fun _ -> run()) //, [||])

    let onClick (ev : Browser.Types.MouseEvent) =
      printfn "raw inputs are %A, %A. inputs are %A" itemCodeOpt treasureClassOpt inputs
      setDropInputs inputs

    Hero.hero [] [
      Button.button [ Button.OnClick onClick ] [ str "Calculate"]
    ]

