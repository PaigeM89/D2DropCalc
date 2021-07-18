namespace App

module Armors =
    open Browser.Dom

    
    open Fable.SimpleHttp

    let getArmors() = async {
        let response =
            Http.request "/api/armors"
            |> Http.method GET
            |> Http.header (Headers.accept "application/avro")
            |> Http.send
        let! (statusCode, responseText) = Http.get "/api/armors"
        match statusCode with
        | 200 ->
            printfn "Successfully hit armors endpoint"
            return responseText |> Some
        | _ ->
            printfn "Received response code %A from endpoint with response %A" statusCode responseText
            return None
    }

    let buttonId = "#loadArmors"

    let getFirstNode (nodeList : Browser.Types.NodeList) =
        nodeList.Item 0

    let tryGetFirstNode (nodeList : Browser.Types.NodeList) =
        if nodeList.length > 0 then nodeList.Item 0 |> Some else None

    let iterHtmlCollection f (coll : Browser.Types.HTMLCollection) =
        for i in 0..(coll.length - 1) do
            coll.Item i |> f

    let all = document.all
    let body = document.body
    
    printfn "document body is %A" body
    printfn "all has length %A and is %A" (all.length) all
    iterHtmlCollection (fun x -> printfn "item is %A" x) all

    let byId = document.getElementById "loadArmor"
    printfn "byId is %A" byId

    let ele = document.getElementsByName buttonId
    printfn "document element is %A" ele
    printfn "element nodes length is %A" ele.length
    let button = ele |> getFirstNode

    printfn "button is %A" button

    let loadArmorButton = button :?> Browser.Types.HTMLButtonElement

    let outputId = "#armorOutput"
    let outputElement = document.getElementsByName outputId |> getFirstNode

    if not (isNull loadArmorButton) then
        loadArmorButton.onclick <- fun _ ->
            printfn "load armor button clicked"
            let armorsTextOpt = getArmors() |> Async.RunSynchronously
            match armorsTextOpt with
            | Some txt ->
                printfn "Received armors text: %s" (txt.Substring(0, 20))
                outputElement.nodeValue <- txt
                printfn "content should be updated"
            | None ->
                printfn "Unable to load armors text"
    else
        printfn "load armor button is null, skipping button setup"