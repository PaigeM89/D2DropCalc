#r "nuget: FSharp.Data"
#r "nuget: Newtonsoft.Json"
#r "nuget: FsToolkit.ErrorHandling"
//#load "../types/items.fsx"
#r "nuget: Thoth.Json.Net"
#r "../../src/D2DropCalc/bin/Debug/net5.0/D2DropCalc.dll"

open FsToolkit.ErrorHandling
open System
open Thoth.Json.Net
open FSharp.Data
open Newtonsoft.Json
open D2DropCalc.Types.ItemTree
open D2DropCalc.Types.Monsters


let outputDir = __SOURCE_DIRECTORY__ + "/../../json/"
let outputFileName = "monsters.json"
let outputPath = outputDir + outputFileName

let inline isOk (r : Result<'a, 'b>) =
    match r with
    | Ok _ -> true
    | Error _ -> false

let inline forceResult (r : Result<'a, 'b>) =
    match r with
    | Ok x -> x
    | Error e -> failwithf "Could not get value from result. Result is Error: %A" e

let maybeGetPropAs (v : JsonValue) (k : string) f =
  match v.TryGetProperty k with
  | Some value -> f value |> Some
  | None -> None

let getPropAs (v : JsonValue) (k : string) f =
  v.GetProperty k |> f

let asInt = fun (jv : JsonValue) -> jv.AsInteger()
let asString = fun (jv : JsonValue) -> jv.AsString()

// **************************
// PARSING
// **************************

[<Literal>]
let monsterPath = __SOURCE_DIRECTORY__ + "/../../input-json/raw/monstats.json"
type Monsters = JsonProvider<monsterPath>

let monsters = Monsters.GetSamples()

let treasureClasses = [
  "TreasureClass1", MonsterDropQuality.Normal, Difficulty.Normal
  "TreasureClass2", MonsterDropQuality.Champion, Difficulty.Normal
  "TreasureClass3", MonsterDropQuality.Unique, Difficulty.Normal
  "TreasureClass4", MonsterDropQuality.Quest, Difficulty.Normal
  "TreasureClass1(N)", MonsterDropQuality.Normal, Difficulty.Nightmare
  "TreasureClass2(N)", MonsterDropQuality.Champion, Difficulty.Nightmare
  "TreasureClass3(N)", MonsterDropQuality.Unique, Difficulty.Nightmare
  "TreasureClass4(N)", MonsterDropQuality.Quest, Difficulty.Nightmare
  "TreasureClass1(H)", MonsterDropQuality.Normal, Difficulty.Hell
  "TreasureClass2(H)", MonsterDropQuality.Champion, Difficulty.Hell
  "TreasureClass3(H)", MonsterDropQuality.Unique, Difficulty.Hell
  "TreasureClass4(H)", MonsterDropQuality.Quest, Difficulty.Hell
]

let tryGetEntryPoint v tcn qual diff =
  match maybeGetPropAs v tcn asString with
  | Some tc ->
    {
      Entrypoint.Quality = qual
      Entrypoint.Diff = diff
      Entrypoint.ItemTreeNode = tc
    } |> Some
  | None -> None

let getName v =
  match maybeGetPropAs v "Name" asString with
  | Some name -> Some name
  | None ->
    maybeGetPropAs v "NameStr" asString

let getLevel v = maybeGetPropAs v "Level" asInt

let getReq v =
  match getName v with
  | Some name ->
    match getLevel v with
    | Some lvl -> Some (name, lvl)
    | None -> None
  | None -> None


// try to create a monster
let tryFrankenstein (v : JsonValue) =
  let req = getReq v
  match req with
  | Some (name, level) ->
    let monsterId = getPropAs v "Id" asString
     //getPropAs v "Name" asString
    //let level = getPropAs v "Level" asInt
    let levelNightmare = maybeGetPropAs v "Level(N)" asInt
    let levelHell = maybeGetPropAs v "Level(H)" asInt
    let entrypoints =
      treasureClasses
      |> List.map (fun (tcn, qual, diff) -> tryGetEntryPoint v tcn qual diff)
      |> List.choose id
    {
      Id = monsterId
      Name = name
      Level = level
      LevelNightmare = levelNightmare
      LevelHell = levelHell
      ItemTreeEntrypoints = entrypoints
    } |> Some
  | _ -> None


let calcMonsters = 
  monsters
  |> Array.map (fun x -> x.JsonValue |> tryFrankenstein)

// **************************
// WRITING
// **************************

let sb = Text.StringBuilder()
calcMonsters
|> Array.iter (fun m ->
  match m with
  | Some m ->
    m.Encode().ToString(Formatting.None, [||])
    |> sb.AppendLine
    |> ignore
  | None -> ()
)

System.IO.File.WriteAllText(outputPath, (string sb))
