#r "nuget: FSharp.Data"
#load "../types/atomics.fsx"
#load "../types/treasureClasses.fsx"
open FSharp.Data
open System
open Atomics
open TreasureClasses

// **************************
// PATHS
// **************************

[<Literal>]
let treasureClassInputPath = __SOURCE_DIRECTORY__ +  "/../../input-json/raw/TreasureClassEx.json"

let outputDir = __SOURCE_DIRECTORY__ + "/../../input-json/transformed"
    //"/Users/maxpaige/git/personal/d2data/formatted-json"

let filepath = outputDir + "/treasureclasses.json"

// **************************
// PARSING
// **************************

type TreasureClasses = JsonProvider<treasureClassInputPath>
let treasureClasses = TreasureClasses.GetSamples()

let maybeGold (name: string) = 
    if name.Contains "gld" then
        if name.Contains "mul" then
            let intStr = System.Text.RegularExpressions.Regex.Match(name, @"\d+").Value;
            let value = Int32.Parse(intStr)
            Gold (Some value) |> Some
        else
            Gold None |> Some
    else
        None

let maybeParseAtomicTcName name = AtomicClass.maybeFromName name

let populateTC (tco : TreasureClasses.Root) =
    let probAndDrop (prob : int option) (item : string option) =
        match item with
        | Some item ->
            match maybeParseAtomicTcName item with
            | Some atomic -> atomic.ToName() |> AtomicClass |> Some
            | None -> 
                match maybeGold item with
                | Some gold -> Some gold
                | None -> TreasureClass item |> Some
        | _ -> None
        |> Option.bind (fun cat ->
            match prob with
            | Some p ->
                {
                    TreasureClassDrop.Category = cat
                    TreasureClassDrop.Probability = p
                } |> Some
            | None -> None
        )
    // should always be populated. may often be gold.
    let drop1 = probAndDrop (Some tco.Prob1) (Some tco.Item1)
    // these may or may not be populated
    let drop2 = probAndDrop tco.Prob2 tco.Item2
    let drop3 = probAndDrop tco.Prob3 tco.Item3
    let drop4 = probAndDrop tco.Prob4 tco.Item4
    let drop5 = probAndDrop tco.Prob5 tco.Item5
    let drop6 = probAndDrop tco.Prob6 tco.Item6
    let drop7 = probAndDrop tco.Prob7 tco.Item7
    let drop8 = probAndDrop tco.Prob8 tco.Item8
    let drop9 = probAndDrop tco.Prob9 tco.Item9
    let drops =
        [ drop1; drop2; drop3; drop4; drop5; drop6; drop7; drop8; drop9 ]
        |> List.choose id
    {
        TreasureClassName = tco.TreasureClass
        Group = tco.Group
        Level = tco.Level
        UniqueModifier = tco.Unique
        SetModifier = tco.Set
        RareModifier = tco.Rare
        MagicModifier = tco.Magic
        Picks = tco.Picks
        NoDrop = Option.defaultValue 0 tco.NoDrop
        Drops = drops
    }

let tcs =
    treasureClasses
    |> Array.map populateTC

// **************************
// WRITING
// **************************

open Newtonsoft.Json

let sb = Text.StringBuilder()
tcs
|> Array.map (fun x -> x.Encode())
|> Array.iter( fun s -> sb.AppendLine s |> ignore )

System.IO.File.WriteAllText(filepath, (sb.ToString()))
printfn "File created!"