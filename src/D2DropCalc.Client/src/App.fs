namespace App

open Fable.Core
open Browser.Dom

module Fetch =
    open Fable.SimpleHttp

    let getArmors() = async {
        let! (statusCode, responseText) = Http.get "/api/armors"
        match statusCode with
        | 200 ->
            printfn "Successfully hit armors endpoint"
            return responseText |> Some
        | _ ->
            printfn "Received response code %A from endpoint with response %A" statusCode responseText
            return None
    }

module App =

    

    // // Get a reference to our button and cast the Element to an HTMLButtonElement
    // let myButton = document.querySelector(".my-button") :?> Browser.Types.HTMLButtonElement

    let test x y = System.Math.Pow(x, y)

    let buttonId = "#loadArmors"
    printfn "Attempting to load by id %s" buttonId

    let getFirstNode (nodeList : Browser.Types.NodeList) =
        nodeList.Item 0

    let tryGetFirstNode (nodeList : Browser.Types.NodeList) =
        if nodeList.length > 0 then nodeList.Item 0 |> Some else None

    let button = document.getElementsByName buttonId |> getFirstNode

    let loadArmorButton = button :?> Browser.Types.HTMLButtonElement

    let outputId = "#armorOutput"
    let outputElement = document.getElementsByName outputId |> getFirstNode

    loadArmorButton.onclick <- fun _ ->
        printfn "load armor button clicked"
        let armorsTextOpt = Fetch.getArmors() |> Async.RunSynchronously
        match armorsTextOpt with
        | Some txt ->
            printfn "Received armors text: %s" (txt.Substring(0, 20))
            outputElement.nodeValue <- txt
            printfn "content should be updated"
        | None ->
            printfn "Unable to load armors text"
        



