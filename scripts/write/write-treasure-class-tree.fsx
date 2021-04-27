#r "nuget: FSharp.Data"
#r "nuget: Newtonsoft.Json"
#load "../types/treasureClasses.fsx"
#load "../types/atomics.fsx"
#r "../../src/D2DropCalc/bin/Debug/net5.0/D2DropCalc.dll"
open System
open Newtonsoft.Json
open Atomics
open TreasureClasses

let inline isOk (r : Result<'a, 'b>) =
    match r with
    | Ok _ -> true
    | Error _ -> false

let inline forceResult (r : Result<'a, 'b>) =
    match r with
    | Ok x -> x
    | Error e -> failwithf "Could not get value from result. Result is Error: %A" e

// *********************
// DATA LOADING
// *********************

[<Literal>]
let TreasureClassesPath = __SOURCE_DIRECTORY__ + "/../../input-json/transformed/treasureclasses.json"

let fileLines = System.IO.File.ReadAllLines(TreasureClassesPath)
let treasureClassResults = 
    [
        for line in fileLines do
            if line <> "" then yield TreasureClass.Decode line
    ]

let failedTreasureClasses = treasureClassResults |> List.filter (isOk >> not)

let treasureClasses =
    treasureClassResults
    |> List.filter isOk
    |> List.map forceResult

[<Literal>]
let AtomicTCPath = __SOURCE_DIRECTORY__ + "/../../input-json/transformed/atomic.json"

/// These are referenced in the tree but not defined because they're empty
/// so we add them manually so that lookups work in a consistent way
let manualAtomics = 
    [
        {
            AtomicClass = AtomicClass.Bow 21
            Items = []
        } |> Ok
        {
            AtomicClass = Bow 66
            Items = []
        } |> Ok
    ]

let atomicFileLines = System.IO.File.ReadAllLines(AtomicTCPath)
let atomicTcResults =
    [
        for line in atomicFileLines do
            if line <> "" then yield AtomicTreasureClass.Decode line
    ]
    |> List.append manualAtomics

let failedAtomicTcs = atomicTcResults |> List.filter (isOk >> not)

let atomicTcs =
    atomicTcResults
    |> List.filter isOk
    |> List.map forceResult
    |> List.map (fun x -> x.AtomicClass.ToName(), x)
    |> Map.ofList

let findAtomic name = Map.tryFind name atomicTcs

let atomicTcNames = atomicTcs |> Map.toList |> List.map (fun (s, atc) -> atc.AtomicClass.ToName())

let buildItemToAtomics() =
    atomicTcs
    |> Map.toList
    |> List.map (fun x -> snd x)
    |> List.map (fun x ->
        x.Items |> List.map (fun item -> (item.Code, (item, x.AtomicClass)))
    )
    |> List.concat
    |> Map.ofList

let itemAtomics = buildItemToAtomics()
let findItem name = Map.tryFind name itemAtomics

// *********************
// TREE DEFINITION
// *********************

// drop lookups
// 1. item bases
// 2. runes
// 3. jewelry
//      a. jewels
//      b. charms
// 6. keys
// 7. essences
// 8. gems
// the goal of this is to be able to make smarter decisions about the tree as we traverse it

// *********************
// ITEM PARSING & BUILDING
// *********************

open D2DropCalc.Types.ItemTree

let (|IsJewelry|_|) code  =
    match code with
    | "rin" -> Item.Jewelry (code, Ring) |> Some
    | "amu" -> Item.Jewelry (code, Amulet) |> Some
    | "jew" -> Item.Jewelry (code, Jewel) |> Some
    | "cm3" -> Item.Jewelry (code, GrandCharm) |> Some
    | "cm2" -> Item.Jewelry (code, LargeCharm) |> Some
    | "cm1" -> Item.Jewelry (code, SmallCharm) |> Some
    | _ -> None

let isGold code =
    if code = "gld" then
        Gold 1 |> Some
    elif code.Contains "gld" then
        let intStr = System.Text.RegularExpressions.Regex.Match(code, @"\d+").Value;
        let value = Int32.Parse(intStr)
        Gold value |> Some
    else
        None

let (|IsGold|_|) code = isGold code

let isRune code = 
    let runeStr = System.Text.RegularExpressions.Regex.Match(code, @"r\d\d$")
    if runeStr.Length > 0 then
        Rune runeStr.Value |> Some
    else
        None

let (|IsRune|_|) code = isRune code

let isRuneTree code =
    let runeTreeStr = System.Text.RegularExpressions.Regex.Match(code, @"Runes \d{1,2}$")
    if runeTreeStr.Length > 0 then
        RunesReference (runeTreeStr.Value, 1) |> Some
    else
        None

let (|IsRuneTree|_|) code = isRuneTree code

let gemCodes = 
    [ "gcv";  "gcy";  "gcb";
    "gcg";  "gcr";  "gcw";
    "skc";  "gfv";  "gfy";
    "gfb";  "gfg";  "gfr";
    "gfw";  "skf";  "gsv";
    "gsy";  "gsb";  "gsg";
    "gsr";  "gsw";  "sku";
    "gzv";  "gly";  "glb";
    "glg";  "glr";  "glw";
    "skl";  "gpv";  "gpy";
    "gpb";  "gpg";  "gpr";
    "gpw";  "skz"]

let isGem code =
    if List.contains code gemCodes then
        Gem code |> Some
    else
        None

let (|IsGem|_|) code = isGem code

let essences = [
    "ceh"; "tes"; "bet"; "fed"
]

let isEssence code = if List.contains code essences then Essence code |> Some else None

let (|IsEssence|_|) code = isEssence code


let potions = [
    "hp1"; "hp2"; "hp3"; "hp4"; "hp5"
    "mp1"; "mp2"; "mp3"; "mp4"; "mp5"
    "rvs"; "rvl"; "yps"; "wms"; "vps"
]

let isPotion code =
    if List.contains code potions then
        Item.Potion code |> Some
    else None

let (|IsPotion|_|) code = isPotion code

let keys = [ "pk1"; "pk2"; "pk3"; "key" ]
let isKey code = if List.contains code keys then Keys code |> Some else None
let (|IsKey|_|) code = isKey code

let ammo = [ "aqv"; "cqv" ]
let scrolls = [ "tsc"; "isc" ]
let unused = [ "gps"; ]
let throwables = [ "opl"; "ops"; "gpl"; "gps"; "gpm"; "opm"; ]

let isJunk code =
    let isAmmo = List.contains code ammo
    let isScroll = List.contains code scrolls
    let isUnused = List.contains code unused
    let isThrowable = List.contains code throwables
    if isAmmo || isScroll || isUnused || isThrowable then Item.Junk code |> Some else None

let (|IsJunk|_|) code = isJunk code

let ingredients = [ "dhn"; "bey"; "mbr" ]
let isIngredient code = if List.contains code ingredients then Ingredient code |> Some else None
let (|IsIngredient|_|) code = isIngredient code

let isAnnihilus (code : string) = if code.ToLowerInvariant() = "annihilus" then Annihilus code |> Some else None
let (|IsAnnihilus|_|) code = isAnnihilus code


// *********************
// TREASURE CLASS PARSING
// *********************

open System.Text.RegularExpressions


let parseAct (str : string) =
    let numberToAct num = 
        match num with
        | "1" -> Act1
        | "2" -> Act2
        | "3" -> Act3
        | "4" -> Act4
        | "5" -> Act5
        | _ -> failwithf "cannot match %s to act" num
    let regex = @"Act (\d)"
    let regMatch = Regex.Match(str, regex)
    let groups = regMatch.Groups.Values |> Seq.toList
    match groups with
    | [_; act ] ->
        let act = numberToAct act.Value
        act
    | _ ->
        failwithf "Unable to parse act from input %s" str

let parseDifficulty (str : string) =
    if str.Contains "(H)" then Hell
    elif str.Contains "(N)" then Nightmare
    else Normal

let parseTreasureClassType name =
    let regexMatch input target = Regex.Match(input, target).Success
    let parseActAndDiff s = parseAct s, parseDifficulty s
    
    let (|IsAmmo|_|) v =
        if regexMatch v @"Ammo" then
            Ammo |> Some
        else None
    let (|IsMisc|_|) v =
        if regexMatch v @"Misc \d" then
            Misc |> Some
        else None
    let (|IsGems|_|) v =
        if regexMatch v @".* Gem" then
            Gems |> Some
        else None
    let (|IsJewelry|_|) v =
        if regexMatch v @"Jewelry .$" then
            Jewelry |> Some
        else None
    let (|IsPotions|_|) v =
        if regexMatch v @"H?[pP]otion \d" then
            Potions |> Some
        else None
    let (|IsCpot|_|) v =
        if regexMatch v @".* Cpot" then
            let act, diff = parseActAndDiff v
            CPot (act, diff) |> Some
        else None
    let (|IsJunk|_|) v =
        if regexMatch v @".* Junk$" then
            let act, diff = parseActAndDiff v
            Junk (act, diff) |> Some
        else None
    let (|IsGood|_|) v =
        if regexMatch v @".* Good$" then
            let act, diff = parseActAndDiff v
            Good (act, diff) |> Some
        else
            None
    let (|IsEquip|_|) v =
        if regexMatch v @".* Equip .$" then
            let act, diff = parseActAndDiff v
            Equipment (act, diff) |> Some
        else None
    let (|IsMelee|_|) v =
        if regexMatch v @".* Melee .$" then
            let act, diff = parseActAndDiff v
            Melee (act, diff) |> Some
        else None
    let (|IsBows|_|) v =
        if regexMatch v @".* Bow .$" then
            let act, diff = parseActAndDiff v
            Bows (act, diff) |> Some
        else None
    let (|IsMagic|_|) v =
        if regexMatch v @".* Magic .$" then
            let act, diff = parseActAndDiff v
            Magic (act, diff) |> Some
        else None
    let (|IsBoss|_|) v =
        if regexMatch v @"(Duriel|Andariel|Mephisto|Baal|Diablo)q?" then
            let diff = parseDifficulty v
            let isQuest = regexMatch v @"(Duriel|Andariel|Mephisto|Baal|Diablo)q"
            Boss (diff, isQuest) |> Some
        else None
    let (|IsCountessRune|_|) v =
        if regexMatch v @"Countess Rune.*" then
            let diff = parseDifficulty v
            CountessRune (diff) |> Some
        else None
    let (|IsCountessItem|_|) v =
        if regexMatch v @"Countess Item.*" then
            let diff = parseDifficulty v
            CountessItem (diff) |> Some
        else None
    let (|IsUItem|_|) v =
        if regexMatch v @".* Uitem .*" then
            let act,diff = parseActAndDiff v
            UItem (act, diff) |> Some
        else None
    let (|IsCItem|_|) v =
        if regexMatch v @".* Citem .*" then
            let act,diff = parseActAndDiff v
            CItem (act, diff) |> Some
        else None

    match name with
    | IsAmmo tct -> Some tct
    | IsMisc tct -> Some tct
    | IsGems tct -> Some tct
    | IsJewelry tct -> Some tct
    | IsPotions tct -> Some tct
    | IsCpot tct -> Some tct
    | IsJunk tct -> Some tct
    | IsGood tct -> Some tct
    | IsEquip tct -> Some tct
    | IsMelee tct -> Some tct
    | IsBows tct -> Some tct
    | IsMagic tct -> Some tct
    | IsBoss tct -> Some tct
    | IsCountessRune tct -> Some tct
    | IsCountessItem tct -> Some tct
    | IsUItem tct -> Some tct
    | IsCItem tct -> Some tct
    | _ ->
        printfn "Unable to map '%s' to a treasure class type" name
        None

let (|IsTreasureClassType|_|) v =
    match parseTreasureClassType v with
    | Some x -> TreasureClassReference (v, x, 1) |> Some
    | None -> None

// *********************
// TREE BUILDING
// *********************

type TreasureClassMap = 
| ItemCodeMap of Item
| NodeValueMap of NodeValue
| NoMap

let tryMapTreasureClass name =
    match name with
    | IsJewelry jewelry -> ItemCodeMap jewelry
    | IsGold gld -> ItemCodeMap gld
    | IsRune r -> ItemCodeMap r
    | IsGem g -> ItemCodeMap g
    | IsPotion p -> ItemCodeMap p
    | IsEssence e -> ItemCodeMap e
    | IsKey k -> ItemCodeMap k
    | IsJunk j -> ItemCodeMap j
    | IsRuneTree rt -> NodeValueMap rt
    | IsIngredient i -> ItemCodeMap i
    | IsAnnihilus a -> ItemCodeMap a
    | IsTreasureClassType tct -> NodeValueMap tct
    | _ ->
        printfn "unable to map code '%s' to an item or a treasure class type" name
        NoMap


let mapItemCode (code : string) =
    match findItem code with
    | Some (aic, ac) ->
        let prob = aic.Probability
        let level = ac.Level()
        let baseItem =
            match ac with
            | AtomicClass.Weapon _ -> Weapon (aic.Code)
            | AtomicClass.Melee _ -> ItemType.Melee (aic.Code)
            | AtomicClass.Bow _ -> ItemType.Bow (aic.Code)
            | AtomicClass.Armor _ -> Armor (aic.Code)
        (Base (baseItem, level), prob)
        |> Some
    | None -> 
        printfn "Unable to find item for code %s" code
        None

let buildItemCollection (atomic : AtomicTreasureClass) =
    let items = 
        atomic.Items
        |> List.map (fun item ->
            mapItemCode item.Code
        ) |> List.choose id
    let name, level = atomic.AtomicClass.ToFullName()
    let code = atomic.AtomicClass.ToName()
    {
        Name = name
        Level = level
        Code = code
        Items = items
    }

let setNodeValueProbability nv prob =
    match nv with
    | ItemCollection (coll, _) -> ItemCollection (coll, prob)
    | ItemCode (ic, _) -> ItemCode (ic, prob)
    | TreasureClassReference (name, tct, _) -> TreasureClassReference (name, tct, prob)
    | UnknownTreasureClassReference (tc, _) -> UnknownTreasureClassReference (tc, prob)
    | RunesReference (runes, _) -> RunesReference (runes, prob)

let buildNodeValue (tcd : TreasureClassDrop) =
    match tcd.Category with
    | AtomicClass ac ->
        let items = 
            match findAtomic ac with
            | Some ac -> buildItemCollection ac
            | None -> failwithf "Unable to find atomic class %s" ac
        ItemCollection (items, tcd.Probability)
    | TreasureClass tcName ->
        match tryMapTreasureClass tcName with
        | ItemCodeMap item -> ItemCode(item, tcd.Probability)
        | NodeValueMap nv -> setNodeValueProbability nv tcd.Probability
        | NoMap ->
            // in rare instances, an item (like a scimitar) will be a direct treasure class drop.
            match mapItemCode tcName with
            | Some (item, _) -> ItemCode(item, tcd.Probability)
            | None ->
                printfn "Creating unknown treasure class reference %s" tcName
                UnknownTreasureClassReference (tcName, tcd.Probability)
    | TreasureClassDropCategory.Gold mult ->
        let i = Item.Gold (Option.defaultValue 1 mult)
        (i, tcd.Probability) |> ItemCode

let buildChances (tc : TreasureClass) =
    let chances = {
        Unique = tc.UniqueModifier
        Set = tc.SetModifier
        Rare = tc.RareModifier
        Magic = tc.MagicModifier
    }
    let allNone (c) =
        c.Unique.IsNone && c.Set.IsNone && c.Rare.IsNone && c.Magic.IsNone

    if allNone chances then None else Some chances

let buildTreeNode (tc : TreasureClass) =
    let name = tc.TreasureClassName
    let noDrop = tc.NoDrop
    let picks = tc.Picks
    let nodes =
        tc.Drops
        |> List.map buildNodeValue
    let tct =
        match tryMapTreasureClass name with
        | ItemCodeMap _ -> None
        | NoMap -> None
        | NodeValueMap (TreasureClassReference (name, tct, _)) ->
            Some tct
        | NodeValueMap _ -> None

    let chances = buildChances tc

    {
        Name = name
        Nodes = nodes
        Picks = picks
        NoDrop = noDrop
        TreasureClassType = tct
        Chances = chances
    }

let nodes = [
    for tc in treasureClasses ->
        buildTreeNode tc
]

// *********************
// WRITING TO FILE
// *********************

let outputDir = __SOURCE_DIRECTORY__ + "/../../json"

let filepath = outputDir + "/treasureclasses.json"

let sb = Text.StringBuilder()
nodes
|> List.iter (fun tcn ->
    tcn.Encode()
    |> sb.AppendLine
    |> ignore
)

System.IO.File.WriteAllText(filepath, (string sb))
printfn "File created!"
