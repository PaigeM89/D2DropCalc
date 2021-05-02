namespace D2DropCalc.SPA.Components

module DropCalcDropdowns =
    open Feliz
    open Feliz.UseDeferred
    open Feliz.SelectSearch
    open Fable.React
    open Fable.React.Props
    open Fable.FontAwesome
    open Fulma
    open D2DropCalc.SPA.Components.Micro.Dropdown
    open D2DropCalc.SPA

    module private Mappers =
        open D2DropCalc.Types.Items
        open D2DropCalc.Types.ItemTree
        open D2DropCalc.Types.Monsters
        
        let itemsLoader() = async {
            let! armors = Fetch.getArmors()
            let! weapons = Fetch.getWeapons()
            match armors, weapons with
            | Ok armors, Ok weapons ->
                let armorDDLValue (armor : Armor) = createDDLValue armor.Code armor.Name
                let weaponDDLValue (weap : Weapon) = createDDLValue weap.Code weap.Name
                return 
                    [
                        SelectOption.Group {
                            name = "Armors"
                            items = armors |> List.map armorDDLValue
                        }
                        SelectOption.Group {
                            name = "Weapons"
                            items = weapons |> List.map weaponDDLValue
                        }
                    ] |> Ok
            | Error e1, Error e2 ->
                printfn $"Fetch errors: %A{e1}, %A{e2}"
                return Error "Unable to load values"//Error "Unable to load values"
            | Error e1, _
            | _, Error e1 ->
                printfn $"Fetch error: %A{e1}"
                return Error "Unable to load values"
        }

        let difficultyLoader() = async {
            return [
                createDDLValue (string Difficulty.Normal) (string Difficulty.Normal)
                createDDLValue (string Difficulty.Nightmare) (string Difficulty.Nightmare)
                createDDLValue (string Difficulty.Hell) (string Difficulty.Hell)
            ] |> Ok
        }

        let difficultyMatcher str diff = 
            match Difficulty.FromString str with
            | Ok x when diff = x -> true
            | _ -> false

        let qualityLoader() =  async {
            return [
                createDDLValue (string MonsterDropQuality.Normal) (string MonsterDropQuality.Normal)
                createDDLValue (string Champion) (string Champion)
                createDDLValue (string Unique) (string Unique)
                createDDLValue (string Quest) (string Quest)
            ] |> Ok
        }

        let qualityMatcher str (selected : SelectItem) =
            match MonsterDropQuality.FromString str with
            | Ok x when selected.name = str -> true
            | _ -> false

        let monsterLoader() =  async {
            let! results = Fetch.getMonsters()
            match results with
            | Ok x -> return Ok x
            | Error e ->
                printfn $"Error fetching monsters: %A{e}"
                return Error "Unable to load monsters"
        }

        let monsterMapper (m : Monster) = Micro.Dropdown.createDDLValue m.Id m.Name
        let monsterMatcher (monsterId : string) (monster : Monster) = monster.Id = monsterId

    type DropdownOutputs = {
        itemCode : string option
        difficulty : D2DropCalc.Types.ItemTree.Difficulty option
        quality : D2DropCalc.Types.Monsters.MonsterDropQuality option
        monster : D2DropCalc.Types.Monsters.Monster option
        treasureClass : string option
    } with
        member this.SetItem itemCode = { this with itemCode = Some itemCode}
        member this.SetDiff diff = { this with difficulty = Some diff }
        member this.SetQuality qual = { this with quality = Some qual }
        member this.SetMonster mon = { this with monster = Some mon }

    let trySetTreasureClass (ddo : DropdownOutputs) =
        match ddo.difficulty, ddo.quality, ddo.monster with
        | Some d, Some q, Some m ->
            let tc =
                m.ItemTreeEntrypoints
                |> List.tryFind (fun ep -> ep.Diff = d && ep.Quality = q)
                |> Option.map (fun x -> x.ItemTreeNode)
            { ddo with treasureClass = tc }
        | _ -> ddo

    let setItem ic (ddo : DropdownOutputs) = ddo.SetItem ic
    let setDiff d (ddo : DropdownOutputs) = ddo.SetDiff d
    let setQual q (ddo : DropdownOutputs) = ddo.SetQuality q
    let setMonster m (ddo : DropdownOutputs) = ddo.SetMonster m


    [<ReactComponent>]
    let AllDropdowns (dropdownOutputsChange : DropdownOutputs -> unit)  =
        let (dropdownOutputs, setDropdownOutputs) = React.useState(None)
        let initIfNone ddo =
            match ddo with
            | Some x -> x
            | None -> {
                itemCode = None
                difficulty = None
                quality = None
                monster = None
                treasureClass = None
            }

        let itemCallback str =
            printfn $"Root setting item code to '%s{str}"
            let ddo = dropdownOutputs |> initIfNone |> setItem str
            setDropdownOutputs (Some ddo)
            dropdownOutputsChange ddo

        
        let diffCallback dStr =
            match D2DropCalc.Types.ItemTree.Difficulty.FromString dStr with
            | Ok d ->
                printfn $"Root setting difficulty to %A{d}"
                let ddo = 
                    dropdownOutputs |> initIfNone |> setDiff d |> trySetTreasureClass
                setDropdownOutputs (Some ddo)
                dropdownOutputsChange ddo
            | Error e ->
                printfn $"Error parsing difficulty from selected value '%s{dStr}' : %A{e}"


        let qualCallback q =
            match D2DropCalc.Types.Monsters.MonsterDropQuality.FromString q with
            | Ok q ->
                printfn $"Root setting monster quality to %A{q}"
                let ddo = dropdownOutputs |> initIfNone |> setQual q |> trySetTreasureClass
                setDropdownOutputs (Some ddo)
                dropdownOutputsChange ddo
            | Error e ->
                printfn $"Error parsing monster quality from selected value '%s{q}' : %A{e}"

        let monsterCallback monster =
            printfn $"Root setting monster id to '%A{monster}"
            let ddo = dropdownOutputs |> initIfNone |> setMonster monster |> trySetTreasureClass
            setDropdownOutputs (Some ddo)
            dropdownOutputsChange ddo


        Columns.columns [] [
            Column.column [] [
                DropdownGrouped (itemCallback, Mappers.itemsLoader, "Select an item")
            ]
            Column.column [ Column.Width (Screen.All, Column.Is4) ] [
                Level.level [] [
                    Dropdown(diffCallback, Mappers.difficultyLoader, id, "Select a difficulty")
                    Dropdown(qualCallback, Mappers.qualityLoader, id, "Select a monster quality")
                ]
            ]
            Column.column [] [
                DropdownWithMatcher(monsterCallback, Mappers.monsterLoader, Mappers.monsterMapper, Mappers.monsterMatcher, "Select a monster")
            ]
        ]
