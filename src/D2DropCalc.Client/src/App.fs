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
                    let output = document.getElementById "armorText" :?> Browser.Types.HTMLParagraphElement
                    
                    printfn "content should be updated"
                | None ->
                    printfn "Unable to load armors text"
            )

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
