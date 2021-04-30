namespace D2DropCalc.Server.Handlers

open D2DropCalc.Server

module ItemTreeHandler =
    open Microsoft.AspNetCore.Http
    open Giraffe

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


    let getArmors next (ctx : HttpContext) =
        printfn "sending armors"
        let loader = ctx.GetService<Loading.IServeData>()
        let armors = loader.Armors() |> List.map (fun x -> x.EncodeMinimal()) |> Thoth.Json.Net.Encode.list
        json armors next ctx

    let getArmorsAsData next (ctx : HttpContext) =
        let loader = ctx.GetService<Loading.IServeData>()
        let armors = loader.Armors() |> D2DropCalc.Types.Items.encodeArmorAsData
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

    module Routes =

        let routes : HttpHandler = choose [
            route "/armors" >=> GET >=> Views.armorsView
            
            route "/api/reload" >=> POST >=> reloadData
            route "/api/armors" >=> GET >=> getArmors
            route "/api/armors/asdata" >=> GET >=> getArmorsAsData
            route "/api/weapons" >=> GET >=> getWeapons
            route "/api/monsters" >=> GET >=> getMonsters
        ]
