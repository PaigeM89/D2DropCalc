namespace D2DropCalc.Types

open System
#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif



/// types relating to base items, and their magical variants
module Items =

    type UniqueItem = {
        Name : string
        BaseItemCode : string
        BaseItemName : string
        ItemLevel : int option
        ReqLevel : int option
        Rarity : int option
    } with
        static member Create name baseCode baseName ilvl rlvl rarity = {
            Name = name
            BaseItemCode = baseCode
            BaseItemName = baseName
            ItemLevel = ilvl
            ReqLevel = rlvl
            Rarity = rarity
        }

        member this.Encode() =
            Encode.object [
                "name", Encode.string this.Name
                "baseItemCode", Encode.string this.BaseItemCode
                "baseItemName", Encode.string this.BaseItemName
                "itemLevel", Encode.option Encode.int this.ItemLevel
                "reqLevel", Encode.option Encode.int this.ReqLevel
                "rarity", Encode.option Encode.int this.Rarity
            ]

        static member Decode() : Decoder<UniqueItem> =
            Decode.object (fun get ->
                {
                    Name = get.Required.Field "name" Decode.string
                    BaseItemCode = get.Required.Field "baseItemCode" Decode.string
                    BaseItemName = get.Required.Field "baseItemName" Decode.string
                    ItemLevel = get.Optional.Field "itemLevel" Decode.int
                    ReqLevel = get.Optional.Field "reqLevel" Decode.int
                    Rarity = get.Optional.Field "rarity" Decode.int
                }
            )

    /// Items come in 3 tiers: normal, exceptional, and elite. This is the "item base" 
    /// and is a rough gauge for effectiveness and requirements.
    type BaseType = 
    | Normal
    | Exceptional
    | Elite
    with
        member this.MinimalString() =
            match this with
            | Normal -> "no"
            | Exceptional -> "ex"
            | Elite -> "el"
        member this.Encode() =
            let str = 
                match this with
                | Normal -> "normal"
                | Exceptional -> "exceptional"
                | Elite -> "elite"
            Encode.string str
        static member FromString str = 
            match str with
            | "normal" -> Normal
            | "exceptional" -> Exceptional
            | "elite" -> Elite
            | _ -> failwithf "Unknown base item type: '%s'" str
        member this.EncodeMinimal() =
            let str = this.MinimalString()
            Encode.string str
        static member FromMinimalString str = 
            match str with
            | "no" -> Normal
            | "ex" -> Exceptional
            | "el" -> Elite
            | _ -> failwithf "Unknown base item type from minimal string: '%s'" str
        override this.ToString() =
            match this with
            | Normal -> "Normal"
            | Exceptional -> "Exceptional"
            | Elite -> "Elite"

    type Weapon = {
        Name : string
        Type : string
        Code : string
        Rarity : int option
        Speed : int option
        ItemLevel : int option
        ReqLevel : int option
        MaxSockets : int option
        ReqStrength : int option
        ReqDex : int option
        BaseType : BaseType
    } with
        static member Create name _type code rarity speed ilvl rlvl sockets str dex _base = {
            Name = name
            Type = _type
            Code = code
            Rarity = rarity
            Speed = speed
            ItemLevel = ilvl
            ReqLevel = rlvl
            MaxSockets = sockets
            ReqStrength = str
            ReqDex = dex
            BaseType = _base
        }

        member this.Encode() =
            Encode.object [
                "name", Encode.string this.Name
                "type", Encode.string this.Type
                "code", Encode.string this.Code
                "rarity", Encode.option Encode.int this.Rarity
                "speed", Encode.option Encode.int this.Speed
                "itemLevel", Encode.option Encode.int this.ItemLevel
                "reqLevel", Encode.option Encode.int this.ReqLevel
                "maxSockets", Encode.option Encode.int this.MaxSockets
                "reqStrength",  Encode.option Encode.int this.ReqStrength
                "reqDex",  Encode.option Encode.int this.ReqDex
                "baseType", (this.BaseType.Encode())
            ]

        member this.EncodeMinimal() =
            Encode.object [
                "n", Encode.string this.Name
                "t", Encode.string this.Type
                "c", Encode.string this.Code
                "r", Encode.option Encode.int this.Rarity
                "s", Encode.option Encode.int this.Speed
                "ilvl", Encode.option Encode.int this.ItemLevel
                "rlvl", Encode.option Encode.int this.ReqLevel
                "ms", Encode.option Encode.int this.MaxSockets
                "rs",  Encode.option Encode.int this.ReqStrength
                "rd",  Encode.option Encode.int this.ReqDex
                "bt", (this.BaseType.EncodeMinimal())
            ]

        static member Decoder() : Decoder<Weapon> =
            Decode.object (fun get ->
                {
                    Name = get.Required.Field "name" Decode.string
                    Type = get.Required.Field "type" Decode.string
                    Code = get.Required.Field "code" Decode.string
                    Rarity = get.Optional.Field "rarity" Decode.int
                    Speed = get.Optional.Field "speed" Decode.int
                    ItemLevel = get.Optional.Field "itemLevel" Decode.int
                    ReqLevel = get.Optional.Field "reqLevel" Decode.int
                    MaxSockets = get.Optional.Field "maxSockets" Decode.int
                    ReqStrength = get.Optional.Field "reqStrength" Decode.int
                    ReqDex = get.Optional.Field "reqDex" Decode.int
                    BaseType = get.Required.Field "baseType" Decode.string |> BaseType.FromString
                }
            )

        static member DecoderMinimal() : Decoder<Weapon> =
            Decode.object (fun get ->
                {
                    Name = get.Required.Field "n" Decode.string
                    Type = get.Required.Field "t" Decode.string
                    Code = get.Required.Field "c" Decode.string
                    Rarity = get.Optional.Field "r" Decode.int
                    Speed = get.Optional.Field "s" Decode.int
                    ItemLevel = get.Optional.Field "ilvl" Decode.int
                    ReqLevel = get.Optional.Field "rlvl" Decode.int
                    MaxSockets = get.Optional.Field "ms" Decode.int
                    ReqStrength = get.Optional.Field "rs" Decode.int
                    ReqDex = get.Optional.Field "rd" Decode.int
                    BaseType = get.Required.Field "bt" Decode.string |> BaseType.FromMinimalString
                }
        )

    type Armor = {
        Name : string
        Code : string
        Rarity : int option
        ItemLevel : int option
        ReqLevel : int option
        ReqStrength : int option
        MaxSockets : int option
        SpeedPenalty : int option
        BaseType : BaseType
    } with
        static member Create name code rarity ilvl rlvl str sockets penalty _base = {
            Name = name
            Code = code
            Rarity = rarity
            ItemLevel = ilvl
            ReqLevel = rlvl
            ReqStrength = str
            MaxSockets = sockets
            SpeedPenalty = penalty
            BaseType = _base
        }

        member this.Encode() =
            Encode.object [
                "name", Encode.string this.Name
                "code", Encode.string this.Code
                "rarity", Encode.option Encode.int this.Rarity
                "itemLevel", Encode.option Encode.int this.ItemLevel
                "reqLevel", Encode.option Encode.int this.ReqLevel
                "reqStrength", Encode.option Encode.int this.ReqStrength
                "sockets", Encode.option Encode.int this.MaxSockets
                "speedPenalty", Encode.option Encode.int this.SpeedPenalty
                "baseType", (this.BaseType.Encode())
            ]

        member this.EncodeMinimal() =
            Encode.object [
                "n", Encode.string this.Name
                "c", Encode.string this.Code
                "r", Encode.option Encode.int this.Rarity
                "ilvl", Encode.option Encode.int this.ItemLevel
                "rlvl", Encode.option Encode.int this.ReqLevel
                "rs", Encode.option Encode.int this.ReqStrength
                "so", Encode.option Encode.int this.MaxSockets
                "sp", Encode.option Encode.int this.SpeedPenalty
                "bt", (this.BaseType.EncodeMinimal())
            ]
        
        member this.EncodeAsData() =
            Encode.array [|
                Encode.string this.Name
                Encode.string this.Code
                Encode.option Encode.int this.Rarity
                Encode.option Encode.int this.ItemLevel
                Encode.option Encode.int this.ReqLevel
                Encode.option Encode.int this.ReqStrength
                Encode.option Encode.int this.MaxSockets
                Encode.option Encode.int this.SpeedPenalty
                (this.BaseType.EncodeMinimal())
            |]

        static member EncodeList (li : Armor list) = 
            Encode.list (li |> List.map (fun a -> a.Encode()))

        static member Decoder() : Decoder<Armor> =
            Decode.object (fun get -> 
                {
                    Name = get.Required.Field "name" Decode.string
                    Code = get.Required.Field "code" Decode.string
                    Rarity = get.Optional.Field "rarity" Decode.int
                    ItemLevel = get.Optional.Field "itemLevel" Decode.int
                    ReqLevel = get.Optional.Field "reqLevel" Decode.int
                    ReqStrength = get.Optional.Field "reqStrength" Decode.int
                    MaxSockets = get.Optional.Field "sockets" Decode.int
                    SpeedPenalty = get.Optional.Field "speedPenalty" Decode.int
                    BaseType = get.Required.Field "baseType" Decode.string |> BaseType.FromString
                })

        static member DecoderMinimal() : Decoder<Armor> = 
            Decode.object (fun get -> 
                {
                    Name = get.Required.Field "n" Decode.string
                    Code = get.Required.Field "c" Decode.string
                    Rarity = get.Optional.Field "r" Decode.int
                    ItemLevel = get.Optional.Field "ilvl" Decode.int
                    ReqLevel = get.Optional.Field "rlvl" Decode.int
                    ReqStrength = get.Optional.Field "rs" Decode.int
                    MaxSockets = get.Optional.Field "so" Decode.int
                    SpeedPenalty = get.Optional.Field "sp" Decode.int
                    BaseType = get.Required.Field "bt" Decode.string |> BaseType.FromMinimalString
                })

    let encodeArmorsAsData (armors : Armor list) =
        let jstr (str : string) = Newtonsoft.Json.Linq.JValue str
        let jinto (i : int option) =
            if i.IsSome then
                Newtonsoft.Json.Linq.JValue(i.Value)
            else
                Newtonsoft.Json.Linq.JValue(Unchecked.defaultof<int>)
        let header = [
            jstr "name"
            jstr "code"
            jstr "rarity"
            jstr "itemLevel"
            jstr "reqStrength"
            jstr "reqLevel"
            jstr "sockets"
            jstr "speedPenalty"
            jstr "baseType"
        ]
        let makeRow (armor : Armor) =
            [
                jstr armor.Name //0
                jstr armor.Code //1
                jinto armor.Rarity //2
                jinto armor.ItemLevel //3
                jinto armor.ReqStrength //4
                jinto armor.ReqLevel //5
                jinto armor.MaxSockets //6
                jinto armor.SpeedPenalty //7
                jstr (armor.BaseType.MinimalString()) //8
            ]

        let armorsEncoded = armors |> List.map makeRow
        header :: armorsEncoded

    let decodeArmorsFromData (data : Newtonsoft.Json.Linq.JValue list list) =
        let fromJstr (jv : Newtonsoft.Json.Linq.JValue) = jv.Value :?> string
        let fromJIntOpt (jv : Newtonsoft.Json.Linq.JValue) = jv.Value :?> int option
        let item ind r = List.item ind r
        match data with
        | [] -> []
        | [x] -> []
        | x :: xs ->
            // skip headers
            xs |> List.map (fun row ->
                {
                    Name = item 0 row |> fromJstr
                    Code = item 1 row |> fromJstr
                    Rarity = item 0 row |> fromJIntOpt
                    ItemLevel = item 0 row |> fromJIntOpt
                    ReqStrength = item 0 row |> fromJIntOpt
                    ReqLevel = item 0 row |> fromJIntOpt
                    MaxSockets = item 0 row |> fromJIntOpt
                    SpeedPenalty = item 0 row |> fromJIntOpt
                    BaseType = BaseType.FromMinimalString (fromJstr (List.item 8 row))
                }
            )

    let decodeArmorsFromDataString (data : string) =
        let item ind l = List.item ind l
        let intOpt (s : string) = 
            try
                if s = "" then None else Int32.Parse s |> Some
            with
            | ex ->
                printfn "Unable to parse %s as int" s
                None

        let decodeRow (s : string) =
            try
                let rowData = s.Split(",") |> List.ofArray
                {
                    Name = item 0 rowData
                    Code = item 1 rowData
                    Rarity = item 2 rowData |> intOpt
                    ItemLevel = item 3 rowData |> intOpt
                    ReqStrength = item 4 rowData |> intOpt
                    ReqLevel = item 5 rowData |> intOpt
                    MaxSockets = item 6 rowData |> intOpt
                    SpeedPenalty = item 7 rowData |> intOpt
                    BaseType = BaseType.FromMinimalString (List.item 8 rowData)
                }
            with
            | ex ->
                printfn $"Unable to decode row '%s{s}':\n%A{ex}"
                raise ex
        
        let data = data.Split("\n") |> List.ofArray
        match data with
        | [] -> []
        | [x] -> [] // discard headers
        | x :: xs ->
            xs
            |> List.map (fun row -> 
                if String.IsNullOrEmpty row then
                    printfn "Skipping empty row"
                    None
                else
                    decodeRow row |> Some
            )
            |> List.choose id

/// Types relating to the recursive item tree
module ItemTree =
    type ItemCode = string
    type Probability = int

    type JewelryType = 
    | Ring
    | Amulet
    | Jewel
    | GrandCharm
    | LargeCharm
    | SmallCharm

    type ItemType =
    | Weapon of code : ItemCode
    | Melee of code : ItemCode
    | Bow of code : ItemCode
    | Armor of code : ItemCode
    with
        member this.Code =
            match this with
            | Weapon code -> code
            | Melee code -> code
            | Bow code -> code
            | Armor code -> code

    let isItemTypeForCode code (itemType : ItemType) = code = itemType.Code

    type Item =
    | Base of itemType : ItemType * level : int
    | Rune of code : ItemCode
    | Jewelry of code : ItemCode * jewelryType : JewelryType
    | Keys of code : ItemCode
    | Essence of code : ItemCode
    | Gem of code : ItemCode
    | Gold of multiplier : int
    | Potion of code : ItemCode
    | Junk of code : ItemCode
    | Ingredient of code : ItemCode
    | Annihilus of code : ItemCode
    with
        member this.Code =
            match this with
            | Base (itemType, _) -> itemType.Code
            | Rune code -> code
            | Jewelry (code, _) -> code
            | Keys code -> code
            | Essence code -> code
            | Gem code -> code
            | Gold _ -> "gld"
            | Potion code -> code
            | Junk code -> code
            | Ingredient code -> code
            | Annihilus code -> code
    
    let isItemForCode code (item : Item) = item.Code = code

    /// A collection of Item Codes and their probabilities to be picked
    type ItemCollection = {
        /// The name of this collection, such as "Armor 54"
        Name : string
        /// The level of this collection, such as "54". This is useful when
        /// determining if this collection needs to be checked at all, based on which 
        /// heuristics are in place.
        Level : int

        /// The code for this collection, such as "armo54"
        Code : string
        /// The items within this collection & their respective probabilities.
        Items : (Item * Probability) list
    } with
        member this.SumProbability() =
            this.Items |> List.sumBy snd

    /// Some treasure classes are associated with a difficulty
    type Difficulty =
    | Normal
    | Nightmare
    | Hell
    with
        override this.ToString() =
            match this with
            | Normal -> "normal"
            | Nightmare -> "nightmare"
            | Hell -> "hell"
        static member FromString str =
            match str with
            | "normal" -> Ok Normal
            | "nightmare" -> Ok Nightmare
            | "hell" -> Ok Hell
            | e -> $"Could not create Difficulty from string '%s{e}'" |> Error


    /// Some treasure classes are associated with a specific act
    type Act =
    | Act1
    | Act2
    | Act3
    | Act4
    | Act5

    /// Treasure classes are grouped into general drop types
    type TreasureClassType =
    | Ammo
    /// As the name implies, various stuff
    | Misc
    /// Gems. Each type of gem (chipped, flawed, etc) has its own treasure class.
    /// Gems are not recursive.
    | Gems
    /// Rings, Amulets, and Charms
    | Jewelry
    | Potions
    /// Potions of all shapes & sizes
    | CPot of act : Act * diff : Difficulty
    /// Potions, Misc, and Ammo
    | Junk of act : Act * diff : Difficulty
    /// Gems, Jewelry, Runes.
    | Good of act : Act * diff : Difficulty
    /// General equipment
    | Equipment of act : Act * diff : Difficulty
    /// Melee weapons.
    | Melee of act : Act * diff : Difficulty
    /// Bows.
    | Bows of act : Act * diff : Difficulty
    /// Scrolls & mana potions
    | Magic of act : Act * diff : Difficulty
    /// A boss-specific treasure class
    | Boss of diff : Difficulty * quest : bool
    /// The countess' specific rune drop treasure class.
    | CountessRune of diff : Difficulty
    /// The countess' other drops
    | CountessItem of diff : Difficulty
    /// A collection of items or other treasure classes.
    | UItem of act : Act * diff : Difficulty
    /// A collection of items or other treasure classes.
    /// I have no idea what the difference is between UItem and CItem.
    | CItem of act : Act * diff : Difficulty
    with
        member this.TypeName =
            match this with
            | Ammo _ -> "Ammo"
            | Misc _ -> "Misc"
            | Gems _ -> "Gems"
            | Jewelry _ -> "Jewelry"
            | Potions _ -> "Potions"
            | CPot (_, _) -> "CPot"
            | Junk (_, _) -> "Junk"
            | Good (_, _) -> "Good"
            | Equipment (_, _) -> "Equipment"
            | Melee (_, _) -> "Melee"
            | Bows (_, _) -> "Bows"
            | Magic (_, _) -> "Magic"
            | Boss (_, _) -> "Boss"
            | CountessRune _ -> "CountessRune"
            | CountessItem _ -> "CountessItem"
            | UItem (_, _) -> "UItem"
            | CItem (_, _) -> "CItem"

    type NodeValue =
    | ItemCollection of collection : ItemCollection * probability : Probability
    /// References an item that is not found in an item collection. An example of this
    /// is in the Rune treasure class tree, where an individual rune is treated like a whole
    /// treasure class.
    | ItemCode of item : Item * probability : Probability
    /// References a Treasure Class lookup, a collection of other treasure classes (making
    /// a recursive structure)
    | TreasureClassReference of name: string * tct : TreasureClassType * probability : Probability
    /// A reference to a treasure class that is not explicitly defined as an item code or a treasure class type
    | UnknownTreasureClassReference of name : string * probability : Probability
    /// References a Runes lookup, an entrypoint or continuation of the rune tree recursion.
    /// Runes themselves are just ItemCodes.
    | RunesReference of runeCode : string * probability : Probability
    with
        member this.Probability =
            match this with
            | ItemCollection (_, p) -> p
            | ItemCode (_, p) -> p
            | TreasureClassReference (_, _, p) -> p
            | UnknownTreasureClassReference (_, p) -> p
            | RunesReference (_, p) -> p

        member this.Level =
            match this with
            | ItemCollection (coll, _) -> coll.Level |> Some
            | _ -> None

    type ChanceModifiers = {
        Unique : int option
        Set : int option
        Rare : int option
        Magic : int option
    } with
        static member Empty = {
            Unique = None
            Set = None
            Rare = None
            Magic = None
        }

    let addChances cm1 cm2 =
        match cm1, cm2 with
        | Some cm1, Some cm2 ->
            let max (io1 : int option) io2 =
                match io1, io2 with
                | Some x, Some y -> Math.Max(x, y) |> Some
                | Some x, None -> x |> Some
                | None, Some y  -> y |> Some
                | None, None -> None
            {
                Unique = max cm1.Unique cm2.Unique
                Set = max cm1.Set cm2.Set
                Rare = max cm1.Rare cm2.Rare
                Magic = max cm1.Magic cm2.Magic
            } |> Some
        | Some cm1, None -> cm1 |> Some
        | None, Some cm2 -> cm2 |> Some
        | None, None -> None

    /// A treasure class containing other treasure classes
    /// AND/OR containing item selection leafs
    type TreasureClassNode = {
        Name : string
        Nodes : NodeValue list
        /// How many selections to make from this node.
        Picks : int
        /// The chance of this node generating no drops.
        NoDrop : int
        /// The type of this treasure class.
        TreasureClassType : TreasureClassType option
        /// Modifiers on the chance of dropping higher rarity items (by magic find, not by base).
        /// Not all treasure classes have a chance modifier, and not all chance modifiers have all values
        Chances : ChanceModifiers option
    } with
        /// the sum of all probabilities in this node, excluding nodrop
        member this.SumNodesProbability() = (this.Nodes |> List.sumBy (fun x -> x.Probability))
        /// the sum of all probabilities in this node, INCLUDING nodrop
        member this.SumAllProbability() =
            this.SumNodesProbability() + this.NoDrop

        member this.Encode() = Encode.Auto.toString(0, this)
        static member Decode str = Decode.Auto.fromString<TreasureClassNode> str
        static member Decoder() = Decode.Auto.generateDecoderCached<TreasureClassNode>()

module Monsters =

    type MonsterDropQuality =
    | Normal
    | Champion
    | Unique
    | Quest
    with
        override this.ToString() =
            match this with
            | Normal -> "normal"
            | Champion -> "champion"
            | Unique -> "unique"
            | Quest -> "quest"

        static member FromString str =
            match str with
            | "normal" -> Ok Normal
            | "champion" -> Ok Champion
            | "unique" -> Ok Unique
            | "quest" -> Ok Quest
            | e -> $"Could not create monster drop quality from string '%s{e}'" |> Error

    type Entrypoint = {
        Quality : MonsterDropQuality
        Diff : ItemTree.Difficulty
        ItemTreeNode : string
    } with
        member this.Encode() =
            Encode.object [
                "quality", Encode.string (this.Quality.ToString())
                "difficulty", Encode.string (this.Diff.ToString())
                "node", Encode.string (this.ItemTreeNode)
            ]

        static member Decoder() : Decoder<Entrypoint> =
            Decode.object (fun get ->
                let qualRes =   get.Required.Field "quality" Decode.string
                                |> MonsterDropQuality.FromString
                let diffRes =   get.Required.Field "difficulty" Decode.string
                                |> ItemTree.Difficulty.FromString
                match qualRes, diffRes with
                | Ok qual, Ok diff ->
                    {
                        Quality = qual
                        Diff = diff
                        ItemTreeNode = get.Required.Field "node" Decode.string
                    }
                | Error e, _
                | _, Error e ->
                    failwith $"Unable to decode entrypoint : '%s{e}'"
            )

    type Monster = {
        Id : string
        Name : string
        Level : int
        LevelNightmare : int option
        LevelHell : int option
        ItemTreeEntrypoints : Entrypoint list
    } with
        member this.Encode() =
            Encode.object [
                "id", Encode.string this.Id
                "name", Encode.string this.Name
                "level", Encode.int this.Level
                "level(N)", Encode.option Encode.int this.LevelNightmare
                "level(H)", Encode.option Encode.int this.LevelHell
                "entrypoints", Encode.list (this.ItemTreeEntrypoints |> List.map (fun x -> x.Encode()))
            ]

        static member Decoder() : Decoder<Monster> =
            Decode.object(fun get ->
                // need to only get successful results
                let entrypoints =
                    get.Required.Field "entrypoints" (Decode.list (Entrypoint.Decoder()))
                {
                    Id = get.Required.Field "id" Decode.string
                    Name = get.Required.Field "name" Decode.string
                    Level = get.Required.Field "level" Decode.int
                    LevelNightmare = get.Optional.Field "level(N)" Decode.int
                    LevelHell = get.Optional.Field "level(H)" Decode.int
                    ItemTreeEntrypoints = entrypoints
                }
            )

    let getEntrypointsForDifficulty diff eps =
        eps |> List.filter (fun x -> snd x = diff)
    let getEntrypointsForQuality qual eps =
        eps |> List.filter (fun x -> fst x = qual)


module DropCalculation =
    open ItemTree

    type CalculationInput = 
    /// the most specific calculation; get the chance for an item from a given root node
    | ItemAndNode of itemCode : ItemCode * nodeName : string
    with

        // override this.ToString() =
        //     match this with
        //     | ItemAndNode (ic, name) -> $"Item And Node: %s{ic}, %s{name}"
        // member this.Encode() = 
        //     Encode.object [
        //         "type", this.ToString()
        //     ]
        member this.Encode() = Encode.Auto.toString(0, this)
        static member Decode str = Decode.Auto.fromString<CalculationInput> str
        static member Decoder() = Decode.Auto.generateDecoderCached<CalculationInput>()

    type DropCalculationError =
    | InsufficientInputs of msg : string
    | MissingTreasureClass of msg : string
    | MissingItemPostCalc of itemCode : ItemCode
    with
        override this.ToString() =
            match this with
            | InsufficientInputs msg -> $"Insufficient inputs to complete the operation. %s{msg}"
            | MissingTreasureClass msg -> $"Treasure class '%s{msg}' not found."
            | MissingItemPostCalc itemCode -> $"Item '%s{itemCode}' not found after calculations."