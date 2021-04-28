namespace D2DropCalc.Server

module Loading =
    open System
    open System.IO
    open D2DropCalc.Types

    [<Literal>]
    let Armors = "armors.json"

    let loadArmors (conf : Config.Config) =
        let dir = conf.JsonDir
        let files = IO.Directory.GetFiles dir
        printfn "All files are %A" files
        files
        |> Array.find (fun x -> x.Contains Armors)
        |> D2DropCalc.Types.Loading.loadArmorsFromPath

    let isOk r =
        match r with
        | Ok _ -> true
        | Error _ -> false

    let forceResult r =
        match r with
        | Ok x -> x
        | Error e -> failwithf "Unable to force result. Error: %A" e

    let parseArmorResults armorResults =
        armorResults
        |> List.filter (isOk >> not)
        |> List.iter (fun x ->
            printfn "Unable to decode armor %A" x
        )

        armorResults
        |> List.filter isOk
        |> List.map forceResult

    type ILoadData =
        abstract member LoadAll : unit -> unit

    type IServeData =
        abstract member Armors : unit -> Items.Armor list

    type DataSource(configServer : Config.IServeConfig) =
        let mutable armors : Items.Armor list = []

        interface ILoadData with
            member this.LoadAll() =
                printfn "Loading all data"
                let conf = configServer.Serve()
                armors <- loadArmors conf |> parseArmorResults

        interface IServeData with
            member this.Armors() = armors
