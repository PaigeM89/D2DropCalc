namespace D2DropCalc.Server.Handlers

open D2DropCalc.Server
open D2DropCalc.Types

module ItemTreeHandler =
    open Microsoft.AspNetCore.Http
    open Giraffe
    // open FsToolkit.ErrorHandling
    // open FsToolkit.ErrorHandling.TaskResult
    open System.Threading.Tasks
    open FSharp.Control.Tasks.Affine

    module Views =
        open Giraffe.ViewEngine

        let armorsView =
            html [] [
                head [] [
                    title []  [ encodedText "D2DropCalc.Server" ]
                    link [ _rel  "stylesheet"
                           _type "text/css"
                           _href "/main.css" ]
                    // if i try doing scripts sent with the page, i use this
                    // script [ _src "scripts/bundle.js"; _type "module" ] [ ]
                ]
                body [] [
                    div [ _id "content" ] [
                        button [ _id "loadArmors" ] [ str "Click to load armors" ]
                        div [ _id "armorOutputDiv" ] [
                            p [ _id "armorText" ] [str "Armors not yet loaded"]
                        ]
                        p [_id "ignore-this"] [ str "this should appear" ]
                    ]
                    p [ _id "ignore-this-2" ] [ str "appears outside the div" ]
                ]
            ] |> htmlView


    let getArmors next (ctx : HttpContext) = task {
        printfn "sending armors"
        match ctx.TryGetRequestHeader "Accept" with
        | Some "application/avro" ->
            let loader = ctx.GetService<Loading.IServeData>()
            let armors = loader.Armors()
            let stream = new System.IO.MemoryStream()
            armors
            |> List.map (fun a -> D2DropCalc.Avro.armorSerializer a stream)
            |> ignore
            ctx.Response.Headers.Add ("Content-Type", Microsoft.Extensions.Primitives.StringValues "application/avro")
            return! ctx.WriteStreamAsync(
                        false,
                        stream,
                        None,
                        None
            )
        | _ ->
            let loader = ctx.GetService<Loading.IServeData>()
            let armors = loader.Armors() |> List.map (fun x -> x.Encode()) |> Thoth.Json.Net.Encode.list
            return! json armors next ctx
    }

    let getArmorsAsData next (ctx : HttpContext) =
        let loader = ctx.GetService<Loading.IServeData>()
        let armors = loader.Armors() |> D2DropCalc.Types.Items.encodeArmorsAsData
        json armors next ctx

    let getWeapons next (ctx : HttpContext) =
        printfn "sending weapons"
        let loader = ctx.GetService<Loading.IServeData>()
        let weapons = loader.Weapons() |> List.map (fun x -> x.EncodeMinimal()) |> Thoth.Json.Net.Encode.list
        json weapons next ctx

    let getMonsters next (ctx : HttpContext) =
        printfn "sending monsters"
        let loader = ctx.GetService<Loading.IServeData>()
        let monsters = loader.Monsters() |> List.map (fun x -> x.Encode()) |> Thoth.Json.Net.Encode.list
        json monsters next ctx

    let reloadData next (ctx : HttpContext) =
        let loader = ctx.GetService<Loading.ILoadData>()
        loader.LoadAll()
        Successful.OK "Reload Complete" next ctx

    let calculateDrop next (ctx : HttpContext) = task {
        let! body = ctx.ReadBodyFromRequestAsync()
        let inputs = Thoth.Json.Net.Decode.fromString (D2DropCalc.Types.DropCalculation.CalculationInput.Decoder()) body
        match inputs with
        | Ok inputs ->
            let dropCalc = ctx.GetService<Loading.ICalculateDrops>()
            let result = dropCalc.CalculateDropForItem inputs
            match result with
            | Ok (itemCode, chance) ->
                return! json chance next ctx
            | Error e ->
                return! ServerErrors.INTERNAL_ERROR (e.ToString()) next ctx
        | Error e ->
            printfn $"Unable to deserialize %s{body}: %A{e}"
            return! RequestErrors.BAD_REQUEST e next ctx
    }

    module Routes =

        let routes : HttpHandler = choose [
            route "/armors" >=> GET >=> Views.armorsView

            routeCi "/api/reload" >=> POST >=> reloadData
            routeCi "/api/armors" >=> GET >=> getArmors
            routeCi "/api/armors/asdata" >=> GET >=> getArmorsAsData
            routeCi "/api/weapons" >=> GET >=> getWeapons
            routeCi "/api/monsters" >=> GET >=> getMonsters
            routeCi "/api/calculatedrop" >=> POST >=> calculateDrop
        ]
