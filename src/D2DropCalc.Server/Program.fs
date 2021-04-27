module D2DropCalc.Server.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe

// ---------------------------------
// Models
// ---------------------------------

type Message =
    {
        Text : string
    }

// ---------------------------------
// Views
// ---------------------------------

module Views =
    open Giraffe.ViewEngine

    let layout (content: XmlNode list) =
        html [] [
            head [] [
                title []  [ encodedText "D2DropCalc.Server" ]
                link [ _rel  "stylesheet"
                       _type "text/css"
                       _href "/main.css" ]
            ]
            body [] content
        ]

    let partial () =
        h1 [] [ encodedText "D2DropCalc.Server" ]

    let index (model : Message) =
        [
            partial()
            p [] [ encodedText model.Text ]
        ] |> layout

// ---------------------------------
// Web app
// ---------------------------------

let isOk r =
    match r with
    | Ok _ -> true
    | Error _ -> false

let forceResult r =
    match r with
    | Ok x -> x
    | Error e -> failwithf "Unable to force result. Error: %A" e

let getArmors next (ctx : HttpContext) =
    let conf = ctx.GetService<Config.IServeConfig>()
    let conf = conf.Serve()
    let armorResults = TreasureClasses.Loading.loadArmors conf
    armorResults
    |> List.filter (isOk >> not)
    |> List.iter (fun x ->
        printfn "Unable to decode armor %A" x
    )
    let response = 
        armorResults
        |> List.filter isOk
        |> List.map forceResult
        |> List.map (fun x -> x.EncodeMinimal())
    json response next ctx

let indexHandler (name : string) =
    let greetings = sprintf "Hello %s, from Giraffe!" name
    let model     = { Text = greetings }
    let view      = Views.index model
    htmlView view

let configHandler next (ctx : HttpContext) =
    let conf = ctx.GetService<Config.IServeConfig>()
    let conf = conf.Serve()
    json conf next ctx

let reloadConfig next (ctx : HttpContext) =
    printfn "In reload config endpoint"
    let conf = ctx.GetService<Config.ILoadConfig>()
    let conf = conf.Load()
    json conf next ctx

let webApp =
    choose [
        choose [
            route "/" >=> GET >=> indexHandler "world"
            GET >=> routef "/hello/%s" indexHandler
            route "/config" >=> choose [
                POST >=> reloadConfig
                GET >=> configHandler
            ]
            route "/armors" >=> GET >=> getArmors
        ]
        setStatusCode 404 >=> text "Not Found"
    ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder : CorsPolicyBuilder) =
    builder
        .WithOrigins(
            "http://localhost:5000",
            "https://localhost:5001")
       .AllowAnyMethod()
       .AllowAnyHeader()
       |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
    (match env.IsDevelopment() with
    | true  ->
        app.UseDeveloperExceptionPage()
    | false ->
        app .UseGiraffeErrorHandler(errorHandler)
            .UseHttpsRedirection())
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let initConfigServer() =
    let conf = LoadJson.ConfigServer()
    let x = conf :> Config.ILoadConfig
    x.Load() |> ignore
    conf

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore
    let conf = initConfigServer()
    
    services.AddSingleton<Config.IServeConfig, LoadJson.ConfigServer>(fun _ -> conf)
        .AddSingleton<Config.ILoadConfig, LoadJson.ConfigServer>(fun _ -> conf)
        |> ignore

let configureLogging (builder : ILoggingBuilder) =
    builder.AddConsole()
           .AddDebug() |> ignore

let buildWebHost args contentRoot webRoot =
    Microsoft.AspNetCore.WebHost.CreateDefaultBuilder(args)
        .UseKestrel()
        .UseContentRoot(contentRoot)
        .UseWebRoot(webRoot)
        .Configure(configureApp)
        .ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging)


[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    let webHost = 
        let h = buildWebHost args contentRoot webRoot
        h.Build()
        // Host.CreateDefaultBuilder(args)
        //     .ConfigureWebHostDefaults(
        //         fun webHostBuilder ->
        //             webHostBuilder
        //                 .UseContentRoot(contentRoot)
        //                 .UseWebRoot(webRoot)
        //                 .Configure(Action<IApplicationBuilder> configureApp)
        //                 .ConfigureServices(configureServices)
        //                 .ConfigureLogging(configureLogging)
        //                 |> ignore)
        //     .Build()
    webHost.Start()
    printfn "Webhost started, ready for requests."
    webHost.WaitForShutdown()
    0