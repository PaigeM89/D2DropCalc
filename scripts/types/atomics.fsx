#r "nuget: Thoth.Json.Net"
open System
open Thoth.Json.Net

let parseAtomicTcName (s : string) =
    let isNumber (c : char) = Char.IsNumber c
    let rec traverse current acc=
        match current with
        | [] -> acc |> List.rev, 0
        | x :: xs when isNumber x ->
            let ns = new String(x :: xs |> Array.ofList)
            let n = ns |> Int32.Parse
            acc |> List.rev, n
        | x :: xs -> traverse xs (x :: acc)
    let allChars = s.ToCharArray(0, s.Length) |> List.ofArray
    let chars, num = traverse allChars []
    let name = new String(chars |> Array.ofList)
    name, num

type AtomicClass = 
| Weapon of level : int
| Melee of level : int
| Bow of level : int
| Armor of level : int
with
    static member maybeFromName name =
        try
            let _class, level = parseAtomicTcName name
            match _class with
            | "weap" -> Weapon level |> Some
            | "mele" -> Melee level |> Some
            | "bow" -> Bow level |> Some
            | "armo" -> Armor level |> Some
            | _ -> None
        with
        | ex ->
            printfn "Error parsing name: %A" ex
            None

    static member fromName name =
        let _class, level = parseAtomicTcName name
        match _class with
        | "weap" -> Weapon level
        | "mele" -> Melee level
        | "bow" -> Bow level
        | "armo" -> Armor level
        | _ -> failwithf "Unable to parse atomic class from name %s" name

    member this.ToName() =
        match this with
        | Weapon lvl -> $"weap{lvl}"
        | Melee lvl -> $"mele{lvl}"
        | Bow lvl -> $"bow{lvl}"
        | Armor lvl -> $"armo{lvl}"

    member this.ToFullName() =
        match this with
        | Weapon lvl -> "Weapon", lvl
        | Melee lvl -> "Melee", lvl
        | Bow lvl -> "Bow", lvl
        | Armor lvl -> "Armor", lvl

    member this.Level() =
        match this with
        | Weapon lvl -> lvl
        | Melee lvl -> lvl
        | Bow lvl -> lvl
        | Armor lvl -> lvl

type AtomicItemChance = {
    /// 3 letter code denoting the item base (eg, "dre" for "Sky Spirit")
    Code : string
    /// Integer value of this item's chance to be drawn relative to the pool.
    Probability: int
} with
    override this.ToString() = sprintf "\"%s\":%i" this.Code this.Probability

type AtomicTreasureClass = {
    AtomicClass : AtomicClass
    Items : AtomicItemChance list
} with
    member this.TotalProbability() =
        this.Items |> List.sumBy (fun i -> i.Probability)

    member this.TryFindItem itemCode =
        this.Items |> List.tryFind (fun i -> i.Code = itemCode)

    member this.Encode() = Encode.Auto.toString(0, this)

    static member Decode str = Decode.Auto.fromString<AtomicTreasureClass>(str, caseStrategy = CamelCase)

type AtomicMap = Map<string, AtomicTreasureClass>

