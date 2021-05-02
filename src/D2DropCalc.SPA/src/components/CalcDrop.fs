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

  // type CalcDropProps = {
  //   itemCode : string option
  //   difficulty : ItemTree.Difficulty option
  //   quality : Monsters.MonsterDropQuality option
  //   monsterId : string option
  // }

  type CalcDropOutputs = {
    itemCode : string
    chance : float
  } with
    static member Create code chance = {
      itemCode = code
      chance = chance
    }

  let propsToOutputs (codeOpt) chance =
    match codeOpt with
    | Some code -> CalcDropOutputs.Create code chance |> Some
    | None -> None

  let convertPropsToInput (itemCode : string option) (treasureClass) =
    match itemCode, treasureClass with
    | Some x, Some y -> DropCalculation.ItemAndNode (x, y) |> Some
    | _ -> None

  [<ReactComponent>]
  let DropCalc (itemCodeOpt, treasureClassOpt, onCalcChange) =
    let inputs = convertPropsToInput itemCodeOpt treasureClassOpt
    //let (dropChances, setDropChances) = React.useState(None)
        //React.useState(Map.empty)
    let (dropInputs, setDropInputs) = React.useState(inputs)
    let (dropOutputs, setDropOutputs) = React.useState(None)

    let run() =
      async {
        printfn "inputs are %A, drop inputs are %A" inputs dropInputs
        match dropInputs with
        | Some inputs ->
          printfn "requesting calculation for inputs %A" inputs
          let! res = inputs |> Post.sendDropCalculation
          match res with
          | Ok f ->
            printfn "Setting drop outputs"
            do propsToOutputs itemCodeOpt f |> setDropOutputs
            //setDropChances (Some(f))
          | Error e ->
            printfn $"Error calculating drop chances: %A{e}"
            //setDropChances None
        | None -> () // setDropChances None
      }
      |> Async.StartImmediate

    React.useEffect(fun _ -> run())

    match dropOutputs with
    | Some x -> 
      printfn "Drop outputs found, invoking callback"
      onCalcChange x
      printfn "Clearing drop outputs"
      setDropOutputs None
      printfn "clearing drop inputs"
      setDropInputs None // remove drop outputs to prevent looping
    | None -> ()

    let onClick (ev : Browser.Types.MouseEvent) =
      printfn "raw inputs are %A, %A. inputs are %A" itemCodeOpt treasureClassOpt inputs
      if inputs <> dropInputs then setDropInputs inputs

    Hero.hero [] [
      Button.button [ Button.OnClick onClick ] [ str "Calculate"]
    ]

