#r "nuget: FSharp.Data"
#r "nuget: Newtonsoft.Json"
#r "nuget: Thoth.Json.Net"
#load "../types/atomics.fsx"
open FSharp.Data
open System
open Atomics

/// This script formats the raw atomic.json data into something more easily consumed later on.

[<Literal>]
let atomicPath = __SOURCE_DIRECTORY__ + "/../../input-json/raw/atomic.json"

type AtomicTCs = JsonProvider<atomicPath>

let fileContents = System.IO.File.ReadAllText atomicPath

let fileAsJson = JsonValue.Parse fileContents

let parseItemChances (atc : JsonValue) =
    atc.Properties()
    |> Array.map (fun (k, v) -> 
        let value = v.AsInteger()
        { Code = k; Probability = value }
    )


let atomicTCs =
    fileAsJson.Properties()
    |> Array.map(fun (k, v) -> 
        let itemChances = parseItemChances v |> List.ofArray
        let ac = AtomicClass.fromName k
        {
            AtomicClass = ac
            Items = itemChances
        }
    )

let outputDir = __SOURCE_DIRECTORY__ + "/../../input-json/transformed"

let filepath = outputDir + "/atomic.json"

let sb = Text.StringBuilder()
atomicTCs
|> Array.iter (fun atc ->
    atc.Encode()
    |> sb.AppendLine
    |> ignore
)

System.IO.File.WriteAllText(filepath, (string sb))
printfn "File created!"
