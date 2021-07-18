#r "nuget: Avro.FSharp"
#r "nuget: Thoth.Json.Net"
#r "../../src/D2DropCalc/bin/Debug/net5.0/D2DropCalc.dll"

open D2DropCalc.Types.Items
open Avro.FSharp

let armorSchema = D2DropCalc.Avro.armorSchema

printfn "%s" (Schema.toCanonicalString armorSchema)

printfn "Armor schema is %A" armorSchema

let armor = Armor.Create "cap/hat" "cap" (Some 1) (Some 1) None None (Some 2) None Normal

open System.IO

// let arrayBufferWriter = new System.Buffers.ArrayBufferWriter<byte>()
// let stream = new System.Text.Json.Utf8JsonWriter( arrayBufferWriter )
let stream = new MemoryStream()

let serialized = D2DropCalc.Avro.armorSerializer armor stream
// let test2 = arrayBufferWriter.ToString()
// let test = stream.Flush()

let data = stream.ToArray()
let size = data.Length

// ------------------

let tcSchema = D2DropCalc.Avro.generateSchema<D2DropCalc.Types.ItemTree.TreasureClassNode>()

printfn "%s" (Schema.toCanonicalString tcSchema)

let tcFile =
  __SOURCE_DIRECTORY__ +  "/../../json/armors.json"
  |> File.ReadAllLines

let decoder = D2DropCalc.Types.Items.Armor.Decoder()

let armors = 
  tcFile
  |> Array.map(fun line ->
    Thoth.Json.Net.Decode.fromString decoder line
)

let fileStream = File.Create("./armors.avro")

let serializedArmors =
  armors
  |> Array.map (fun armor ->
    match armor with
    | Ok armor ->
      D2DropCalc.Avro.armorSerializer armor fileStream
    | Error _ -> ()
  )

fileStream.Flush()