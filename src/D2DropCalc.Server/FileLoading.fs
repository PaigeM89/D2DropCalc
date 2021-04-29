namespace D2DropCalc.Server

module Loading =
    open System
    open System.IO
    open D2DropCalc.Types
    open Thoth.Json.Net

    [<Literal>]
    let Armors = "armors.json"
    [<Literal>]
    let Weapons = "weapons.json"

    let loadTreasureClassesFromPath path =
        let lines = File.ReadAllLines path
        [ 
            for line in lines do 
                if line <> "" then
                    ItemTree.TreasureClassNode.Decode line
        ]

    let loadArmorsFromPath path =
        let lines = File.ReadAllLines path
        let decoder = Items.Armor.Decoder()
        [
            for line in lines do
                if line <> "" then
                    Decode.fromString decoder line
        ]

    let loadWeaponsFromPath path =
        let lines = File.ReadAllLines path
        let decoder = Items.Weapon.Decoder()
        [
            for line in lines do
                if line <> "" then
                    Decode.fromString decoder line
        ]

    let loadArmors (conf : Config.Config) =
        let dir = conf.JsonDir
        let files = IO.Directory.GetFiles dir
        printfn "All files are %A" files
        files
        |> Array.find (fun x -> x.Contains Armors)
        |> loadArmorsFromPath

    let loadWeapons (conf : Config.Config) =
        let dir = conf.JsonDir
        let files = IO.Directory.GetFiles dir
        printfn "All files are %A" files
        files
        |> Array.find (fun x -> x.Contains Weapons)
        |> loadWeaponsFromPath

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

    let parseWeaponResults weaponResults =
        weaponResults
        |> List.filter (isOk >> not)
        |> List.iter (fun x ->
            printfn "Unable to decode weapon %A" x
        )

        weaponResults
        |> List.filter isOk
        |> List.map forceResult

    type ILoadData =
        abstract member LoadAll : unit -> unit

    type IServeData =
        abstract member Armors : unit -> Items.Armor list
        abstract member Weapons : unit -> Items.Weapon list

    type DataSource(configServer : Config.IServeConfig) =
        let mutable armors : Items.Armor list = []
        let mutable weapons : Items.Weapon list = []

        interface ILoadData with
            member this.LoadAll() =
                printfn "Loading all data"
                let conf = configServer.Serve()
                armors <- loadArmors conf |> parseArmorResults
                weapons <- loadWeapons conf |> parseWeaponResults

        interface IServeData with
            member this.Armors() = armors
            member this.Weapons() = weapons
