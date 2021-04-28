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

    printfn "hello world from updated script!"

    // // Get a reference to our button and cast the Element to an HTMLButtonElement
    // let myButton = document.querySelector(".my-button") :?> Browser.Types.HTMLButtonElement

    let afterloading() =
        let btn = document.getElementById "loadArmors" :?> Browser.Types.HTMLButtonElement
        btn.onclick <- fun _ ->
            printfn "load armor button clicked"
            let fetch = Fetch.getArmors()
            let asPromise = fetch |> Async.StartAsPromise
            asPromise |> Promise.map (fun txtOpt ->
                match txtOpt with
                | Some txt ->
                    printfn "Received armors text: %s" (txt.Substring(0, 20))
                    let output = document.getElementById "armorText"
                    
                    printfn "content should be updated"
                | None ->
                    printfn "Unable to load armors text"
            )
            // let armorsTextOpt = asPromise |> Async.AwaitPromise
            // match armorsTextOpt with
            // | Some txt ->
            //     printfn "Received armors text: %s" (txt.Substring(0, 20))
            //     let output = document.getElementById "armorText"
            //     output.nodeValue <- txt
            //     printfn "content should be updated"
            // | None ->
            //     printfn "Unable to load armors text"

        

    let testPow x y = System.Math.Pow(x, y)
    Fable.Core.JsInterop.exportDefault testPow
    
    let state = document.readyState
    printfn "ready state is %A" state
    document.onreadystatechange <- (fun pe ->
        printfn "progress event is %A. as string: %s" pe (pe.ToString())
        printfn "peloaded %f" pe.loaded
        afterloading()
        ()
    )

    // let url = document.URL
    // printfn "current URL is %A" url

    // let buttonId = "#loadArmors"
    // printfn "Attempting to load by id %s" buttonId

    // let getFirstNode (nodeList : Browser.Types.NodeList) =
    //     nodeList.Item 0

    // let tryGetFirstNode (nodeList : Browser.Types.NodeList) =
    //     if nodeList.length > 0 then nodeList.Item 0 |> Some else None

    // let iterHtmlCollection f (coll : Browser.Types.HTMLCollection) =
    //     for i in 0..(coll.length - 1) do
    //         coll.Item i |> f

    // let all = document.all
    // let body = document.body
    
    // printfn "document body is %A" body
    // printfn "all has length %A and is %A" (all.length) all
    // iterHtmlCollection (fun x -> printfn "item is %A" x) all

    // let byId = document.getElementById "loadArmor"
    // printfn "byId is %A" byId

    // let ele = document.getElementsByName buttonId
    // printfn "document element is %A" ele
    // printfn "element nodes length is %A" ele.length
    // let button = ele |> getFirstNode

    // printfn "button is %A" button

    // let loadArmorButton = button :?> Browser.Types.HTMLButtonElement

    // let outputId = "#armorOutput"
    // let outputElement = document.getElementsByName outputId |> getFirstNode

    // if not (isNull loadArmorButton) then
    //     loadArmorButton.onclick <- fun _ ->
    //         printfn "load armor button clicked"
    //         let armorsTextOpt = Fetch.getArmors() |> Async.RunSynchronously
    //         match armorsTextOpt with
    //         | Some txt ->
    //             printfn "Received armors text: %s" (txt.Substring(0, 20))
    //             outputElement.nodeValue <- txt
    //             printfn "content should be updated"
    //         | None ->
    //             printfn "Unable to load armors text"
    // else
    //     printfn "load armor button is null, skipping button setup"

//    let self = Browser.Dom.self
    

