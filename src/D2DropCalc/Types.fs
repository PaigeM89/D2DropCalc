namespace D2DropCalc.Types

open System
open Thoth.Json.Net

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

        member this.Decode() =
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
    } with
        static member Create name _type code rarity speed ilvl rlvl sockets str dex = {
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
            ]

        member this.Decode str =
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
    } with
        static member Create name code rarity ilvl rlvl str sockets penalty = {
            Name = name
            Code = code
            Rarity = rarity
            ItemLevel = ilvl
            ReqLevel = rlvl
            ReqStrength = str
            MaxSockets = sockets
            SpeedPenalty = penalty
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
            ]

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
            this.Items |> List.sumBy (fun x -> snd x)

    /// Some treasure classes are associated with a difficulty
    type Difficulty =
    | Normal
    | Nightmare
    | Hell

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
        Unique : int
        Set : int
        Rare : int
        Magic : int
    } with
        static member (+) (cm1, cm2) = {
            Unique = Math.Max(cm1.Unique, cm2.Unique)
            Set = Math.Max(cm1.Set, cm2.Set)
            Rare = Math.Max(cm1.Rare, cm2.Rare)
            Magic = Math.Max(cm1.Magic, cm2.Magic)
        }

        static member Empty = {
            Unique = 0
            Set = 0
            Rare = 0
            Magic = 0
        }

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
        Chances : ChanceModifiers
    } with
        /// the sum of all probabilities in this node, excluding nodrop
        member this.SumNodesProbability = (this.Nodes |> List.sumBy (fun x -> x.Probability))
        /// the sum of all probabilities in this node, INCLUDING nodrop
        member this.SumAllProbability =
            (this.Nodes |> List.sumBy (fun x -> x.Probability)) + this.NoDrop

    module Loading =
        open System
        open System.IO
        open Newtonsoft.Json

        [<Literal>]
        let path = "/Users/maxpaige/git/personal/d2data/formatted-json/custom-treasure-classes.json"

        let loadCustomTreasureClasses() =
            let lines = File.ReadAllLines path
            [ 
                for line in lines do 
                    if line <> "" then
                        let tcn = JsonConvert.DeserializeObject<TreasureClassNode> line
                        yield tcn.Name, tcn
            ] |> Map.ofList

        let loadTreasureClassesFromPath path =
            let lines = File.ReadAllLines path
            [ 
                for line in lines do 
                    if line <> "" then
                        let tcn = JsonConvert.DeserializeObject<TreasureClassNode> line
                        yield tcn.Name, tcn
            ] |> Map.ofList