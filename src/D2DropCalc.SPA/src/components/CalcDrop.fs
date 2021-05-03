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

  // [<ReactComponent>]
  // let DropCalc (itemCodeOpt, treasureClassOpt, onCalcChange) =
  //   let inputs = convertPropsToInput itemCodeOpt treasureClassOpt
  //   let (dropInputs, setDropInputs) = React.useState(inputs)
  //   let (dropOutputs, setDropOutputs) = React.useState(None)

  //   match dropOutputs with
  //   | Some x -> 
  //     printfn "Drop outputs found, invoking callback"
  //     onCalcChange x
  //     printfn "Clearing drop outputs"
  //     setDropOutputs None
  //     printfn "clearing drop inputs"
  //     setDropInputs None // remove drop outputs to prevent looping
  //   | None -> ()

  //   let run() =
  //     async {
  //       printfn "inputs are %A, drop inputs are %A" inputs dropInputs
  //       match dropInputs, dropOutputs with
  //       | Some inputs, None ->
  //         printfn "requesting calculation for inputs %A" inputs
  //         let! res = inputs |> Post.sendDropCalculation
  //         match res with
  //         | Ok f ->
  //           printfn "Setting drop outputs"
  //           //setDropInputs None
  //           // propsToOutputs itemCodeOpt f
  //           // |> Option.map onCalcChange
  //           // |> ignore
  //           do propsToOutputs itemCodeOpt f |> setDropOutputs
  //           ()
  //         | Error e ->
  //           printfn $"Error calculating drop chances: %A{e}"
  //       | _ ->
  //         printfn "skipping calculation"
  //         ()
  //     }
  //     |> Async.StartImmediate

  //   React.useEffect(fun _ -> run())

  //   let onClick (ev : Browser.Types.MouseEvent) =
  //     printfn "raw inputs are %A, %A. inputs are %A" itemCodeOpt treasureClassOpt inputs
  //     if inputs <> dropInputs then setDropInputs inputs

  //   Hero.hero [] [
  //     Button.button [ Button.OnClick onClick ] [ str "Calculate"]
  //   ]

  [<ReactComponent>]
  let CalculateDropButton (itemCodeOpt, treasureClassOpt, onCalc) =
    let inputs = convertPropsToInput itemCodeOpt treasureClassOpt
    let (calcInputs, setCalcInputs) = React.useState(None)
    let (shouldCalculate, setShouldCalculate) = React.useState(false)

    printfn "in component render, shouldcalc is %A and calc inputs are %A" shouldCalculate calcInputs

    let onClick (_ : Browser.Types.MouseEvent) =
      printfn "In button handler onclick"
      if inputs <> calcInputs then
        printfn "calculating drop chances for raw inputs %A, %A" itemCodeOpt treasureClassOpt
        setCalcInputs inputs
        printfn "Turning on shouldCalculate"
        setShouldCalculate true
      else
        ()

    let runEffect() =
      async {
        printfn "in run effect, shouldcalc is %A" shouldCalculate
        if shouldCalculate then
          printfn "in run effect, inputs are %A" inputs
          match inputs with
          | Some inputs ->
            let! results = inputs |> Post.sendDropCalculation
            match results with
            | Ok chance ->
              printfn "Calculated drop chance for %A, %A to be %f" itemCodeOpt treasureClassOpt chance
              printfn "invoking oncalc callback"
              propsToOutputs itemCodeOpt chance |> onCalc
              printfn "turning off ShouldCalculate"
              setShouldCalculate false
            | Error e ->
              printfn $"Error calculating drop chances: %A{e}"
              setShouldCalculate false
          | None ->
            printfn "Unable to calculate from incomplete inputs"
            setShouldCalculate false
        else
          ()
      } |> Async.StartImmediate

    React.useEffect(fun _ -> runEffect())

    Hero.hero [] [
      Button.button [ Button.OnClick onClick] [ str "Calculate" ]
    ]


