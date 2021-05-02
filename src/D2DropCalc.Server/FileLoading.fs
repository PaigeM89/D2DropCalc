namespace D2DropCalc.Server

module Loading =
    open System
    open System.IO
    open D2DropCalc.Types
    open D2DropCalc.Types.DropCalculation
    open Thoth.Json.Net

    let isOk r =
        match r with
        | Ok _ -> true
        | Error _ -> false

    let forceResult r =
        match r with
        | Ok x -> x
        | Error e -> failwithf "Unable to force result. Error: %A" e

    module Items =

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

    module Monsters =
        [<Literal>]
        let MonstersFile = "monsters.json"
        let loadMonsters (config : Config.Config) =
            let dir = config.JsonDir
            let files = IO.Directory.GetFiles dir
            let decoder = Monsters.Monster.Decoder()
            files
            |> Array.find (fun x -> x.Contains MonstersFile)
            |> File.ReadAllLines
            |> Array.map (fun line ->
                match Decode.fromString decoder line with
                | Ok monster -> Some monster
                | Error e ->
                    printfn "Unable to decode monster.\nError: '%s'" e
                    None
            )
            |> Array.choose id
            |> List.ofArray

    module ItemTree =
        [<Literal>]
        let ItemTreeFile = "treasureclasses.json"

        let loadItemTree (config : Config.Config) =
            let dir = config.JsonDir
            let files = IO.Directory.GetFiles dir
            let decoder = D2DropCalc.Types.ItemTree.TreasureClassNode.Decoder()
            files
            |> Array.find (fun x -> x.Contains ItemTreeFile)
            |> File.ReadAllLines
            |> Array.map (fun line ->
                match Decode.fromString decoder line with
                | Ok monster -> Some monster
                | Error e ->
                    printfn "Unable to decode monster.\nError: '%s'" e
                    None
            )
            |> Array.choose id
            |> List.ofArray

    type ILoadData =
        abstract member LoadAll : unit -> unit

    type IServeData =
        abstract member Armors : unit -> Items.Armor list
        abstract member Weapons : unit -> Items.Weapon list
        abstract member Monsters : unit -> Monsters.Monster list
        abstract member TreasureClassNodes : unit -> ItemTree.TreasureClassNode list
        abstract member TreasureClassNodesMap : unit -> Map<string, ItemTree.TreasureClassNode>

    type DataSource(configServer : Config.IServeConfig) =
        let mutable armors : Items.Armor list = []
        let mutable weapons : Items.Weapon list = []
        let mutable monsters : Monsters.Monster list = []
        let mutable nodes : ItemTree.TreasureClassNode list = []
        let mutable nodesMap : Map<string, ItemTree.TreasureClassNode> = Map.empty

        interface ILoadData with
            member this.LoadAll() =
                printfn "Loading all data"
                let conf = configServer.Serve()
                armors <- Items.loadArmors conf |> Items.parseArmorResults
                weapons <- Items.loadWeapons conf |> Items.parseWeaponResults
                monsters <- Monsters.loadMonsters conf
                nodes <- ItemTree.loadItemTree conf
                nodesMap <- nodes |> List.map (fun n -> (n.Name, n)) |> Map.ofList

        interface IServeData with
            member this.Armors() = armors
            member this.Weapons() = weapons
            member this.Monsters() = monsters
            member _.TreasureClassNodes() = nodes
            member _.TreasureClassNodesMap() = nodesMap

    type ICalculateDrops =
        abstract member CalculateDropForItem : CalculationInput -> Result<(string * float), DropCalculationError>

    type DropCalculator(datasource : IServeData) =
        let calcDropForItem inputs =
            //start with the most specific implementation and slowly build out more as needed
            match inputs with
            | ItemAndNode (ic, tc) ->
                let tcMap = datasource.TreasureClassNodesMap()
                match Map.tryFind tc tcMap with
                | Some tc ->
                    let itemMap, nodrop, chanceMods = D2DropCalc.TreeTraversal.traverseTreasureClasses tcMap tc
                    match Map.tryFind ic itemMap with
                    | Some v -> Ok (ic, v)
                    | None -> MissingItemPostCalc ic |> Error
                | None -> MissingTreasureClass tc |> Error
            //| _ -> InsufficientInputs "Require item code and starting treasure class" |> Error

        interface ICalculateDrops with
            member this.CalculateDropForItem (inputs : CalculationInput) =
                calcDropForItem inputs

