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
                    script [ _src "scripts/bundle.js"; _type "application/javascript" ] []
                ]
                body [] [
                    div [ _id "content" ] [
                        button [ _id "loadArmors" ] [ str "Click to load armors" ]
                    ]
                ]
            ] |> htmlView


    let getArmors next (ctx : HttpContext) =
        let loader = ctx.GetService<Loading.IServeData>()
        let armors = loader.Armors() |> List.map(fun x -> x.EncodeMinimal())
        json armors next ctx

    let reloadData next (ctx : HttpContext) =
        let loader = ctx.GetService<Loading.ILoadData>()
        loader.LoadAll()
        Successful.OK "Reload Complete" next ctx

    module Routes =

        let routes : HttpHandler = choose [
            route "/armors" >=> GET >=> Views.armorsView
            route "/api/armors" >=> GET >=> getArmors
            route "/reload" >=> POST >=> reloadData
        ]