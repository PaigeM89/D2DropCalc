#r "nuget: Thoth.Json.Net"
open System
open Thoth.Json.Net


type TreasureClassDropCategory = 
| AtomicClass of name : string
/// References another treasure class by name.
| TreasureClass of name : string
| Gold of multiplier : int option
with
    member this.MatchesName name =
        match this with
        | AtomicClass acName -> name = acName
        | TreasureClass tcName -> name = tcName
        | Gold _ -> name = "gld"

    member this.Name =
        match this with
        | AtomicClass name -> name
        | TreasureClass name -> name
        | Gold _ -> "gld"


type TreasureClassDrop = {
    Category : TreasureClassDropCategory
    Probability : int
} with
    member this.Encode() = Encode.Auto.toString(0, this)

(*
Treasure classes reference other treasure classes/atomic TCs by name/code, and the objects themselves
are not recursive. 
This is because the structure has a lot of repitition, and repeated dictionary lookups will be
significantly better for recursion.
*)

type TreasureClass = {
        /// The name of the treasure class, such as "Act 4 (H) Good"
        TreasureClassName : string
        /// Identifies similar treasure classes
        Group: int option
        /// The level of this treasure class. Used in the TC upgrade process.
        Level : int option
        /// Modifier for the chance to drop a unique item.
        UniqueModifier : int option
        /// Modifier for the chance to drop a set item.
        SetModifier : int option
        /// Modifier for the chance to drop a rare item.
        RareModifier : int option
        /// Modifier for the chance to drop a magic item.
        MagicModifier : int option
        /// How many iterations of the item selection process is used in this treasure class.
        Picks: int
        /// The chance (as an int relative to the other probabilities) that the pick produces
        /// no drop.
        NoDrop : int
        /// <summary>The collection of drops contained in this treasure class</summary>
        Drops : TreasureClassDrop list
    } with
        static member Create tcn p = {
            TreasureClassName = tcn
            Group = None
            Level = None
            UniqueModifier = None
            SetModifier = None
            RareModifier = None
            MagicModifier = None
            Picks = 1
            NoDrop = 0
            Drops = []
        }

        /// The sum of the probability of all the drops in this treasure class.
        /// Does NOT include the "NoDrop" value of the treasure class.
        member this.TotalDropProbability() = 
            this.Drops
            |> List.sumBy (fun x -> x.Probability)

        /// The sum of the probability of all drops in this treasure class, including the NoDrop
        /// probability. Note that the NoDrop is not calculated based on players.
        member this.TotalProbability() =
            (this.TotalDropProbability()) + this.NoDrop

        member this.Encode() = Encode.Auto.toString(0, this)

        static member Decode str = Decode.Auto.fromString<TreasureClass> str

type TreasureClassMap = Map<string, TreasureClass>