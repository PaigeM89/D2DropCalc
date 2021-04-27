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
//open Newtonsoft.Json
open D2DropCalc.Types.Items
//open Items


open Newtonsoft.Json
let outputDir = __SOURCE_DIRECTORY__ + "/../../json/"

let inline isOk (r : Result<'a, 'b>) =
    match r with
    | Ok _ -> true
    | Error _ -> false

let inline forceResult (r : Result<'a, 'b>) =
    match r with
    | Ok x -> x
    | Error e -> failwithf "Could not get value from result. Result is Error: %A" e

// **************************
// UNIQUES
// **************************

[<Literal>]
let uniqueItemInputPath = __SOURCE_DIRECTORY__ + "/../../input-json/raw/UniqueItems.json"

type UniqueItems = JsonProvider<uniqueItemInputPath>

let jv = UniqueItems.GetSample().JsonValue.Properties()

let maybeGetPropAs (v : JsonValue) (k : string) f =
    match v.TryGetProperty k with
    | Some value -> f value |> Some
    | None -> None

let getPropAs (v : JsonValue) (k : string) f =
    v.GetProperty k |> f

let asInt = fun (jv : JsonValue) -> jv.AsInteger()
let asString = fun (jv : JsonValue) -> jv.AsString()

let maybeCreateUniqueItem (v : JsonValue) =
    let code = maybeGetPropAs v "code" asString
    match code with
    | Some code ->
        UniqueItem.Create
            (getPropAs v "index" asString)
            code
            (getPropAs v "*type" asString)
            (maybeGetPropAs v "lvl" asInt)
            (maybeGetPropAs v "lvl req" asInt)
            (maybeGetPropAs v "rarity" asInt)
        |> Some
    | None ->
        None

let items =
    jv
    |> Array.map (fun (k, v) ->
        maybeCreateUniqueItem v
    )
    |> Array.choose id
    |> Array.map (fun i -> i.Encode())

let filepath = outputDir + "uniqueitems.json"

let sb = Text.StringBuilder()
items
|> Array.iter (fun item ->
    item.ToString(Formatting.None, [||])
    |> sb.AppendLine
    |> ignore
)

System.IO.File.WriteAllText(filepath, (string sb))
printfn "Unique Item file created!"

// **************************
// WEAPONS
// **************************

[<Literal>]
let weaponPath = __SOURCE_DIRECTORY__ + "/../../input-json/raw/weapons.json"
type Weapons = JsonProvider<weaponPath>

let weaponProps = Weapons.GetSample().JsonValue.Properties()

let maybeCreateWeapon (v : JsonValue) =
    let code = maybeGetPropAs v "code" asString
    match code with
    | Some code ->
        Weapon.Create
            (getPropAs v "name" asString)
            code
            (getPropAs v "type" asString)
            (maybeGetPropAs v "rarity" asInt)
            (maybeGetPropAs v "speed" asInt)
            (maybeGetPropAs v "level" asInt)
            (maybeGetPropAs v "levelreq" asInt)
            (maybeGetPropAs v "gemsockets" asInt)
            (maybeGetPropAs v "reqStr" asInt)
            (maybeGetPropAs v "reqDex" asInt)
        |> Ok
    | None ->
        printfn "Unable to create weapon from json value %A" v
        Error v

let weaponOutputPath = outputDir + "weapons.json"

let weaponResults =
    weaponProps
    |> Array.map (fun (k, v) ->
        maybeCreateWeapon v
    )

let encodeWpn (w : Weapon) = w.Encode()

let errorWeapons = weaponResults |> Array.filter (isOk >> not)
let weapons =
    weaponResults
    |> Array.filter (fun x -> isOk x)
    |> Array.map (fun x -> forceResult x |> encodeWpn)

let weaponSb = Text.StringBuilder()
weapons
|> Array.iter (fun item ->
    item.ToString(Formatting.None, [||])
    |> weaponSb.AppendLine
    |> ignore
)

System.IO.File.WriteAllText(weaponOutputPath, (string weaponSb))
printfn "Weapon file created!"

// **************************
// ARMORS
// **************************



[<Literal>]
let armorPath = __SOURCE_DIRECTORY__ + "/../../input-json/raw/armor.json"
type Armors = JsonProvider<armorPath>

let armorProps = Armors.GetSample().JsonValue.Properties()

let maybeCreateArmor (v : JsonValue) =
    let code = maybeGetPropAs v "code" asString
    match code with
    | Some code ->
        Armor.Create
            (getPropAs v "name" asString)
            code
            (maybeGetPropAs v "rarity" asInt)
            (maybeGetPropAs v "level" asInt)
            (maybeGetPropAs v "levelreq" asInt)
            (maybeGetPropAs v "reqstr" asInt)
            (maybeGetPropAs v "gemsockets" asInt)
            (maybeGetPropAs v "speed" asInt)
        |> Ok
    | None ->
        printfn "Unable to create weapon from json value %A" v
        Error v

let armorOutputPath = outputDir + "armors.json"

let armorResults =
    armorProps
    |> Array.map (fun (k, v) ->
        maybeCreateArmor v
    )

let encodeArmor (a : Armor) = a.Encode()

let errorArmors = armorResults |> Array.filter (isOk >> not)
let armors =
    armorResults
    |> Array.filter (fun x -> isOk x)
    |> Array.map (fun x -> forceResult x |> encodeArmor)

let armorSb = Text.StringBuilder()
armors
|> Array.iter (fun item ->
    item.ToString(Formatting.None, [||])
    |> armorSb.AppendLine
    |> ignore
)

System.IO.File.WriteAllText(armorOutputPath, (string armorSb))
printfn "Armor file created!"
