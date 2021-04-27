namespace D2DropCalc

module TreeTraversal =
    open System
    open D2DropCalc.Types.ItemTree

    let doesNodeContainItem code node =
        match node with
        | ItemCollection (items, _) -> 
            items.Items
            |> List.map (fun (item, _) -> isItemForCode code item)
            |> List.exists (fun x -> x = true)
        | ItemCode (item, _) -> isItemForCode code item
        | TreasureClassReference (name, _, _) -> code = name
        | UnknownTreasureClassReference (name, _) -> code = name
        | RunesReference (name, _) -> name = code

    let getItemNodes treasureClasses code =
        treasureClasses
        |> Map.toList
        |> List.filter (fun (_, tcn) -> List.map (doesNodeContainItem code) tcn.Nodes |> List.exists (fun x -> x = true) )
        |> List.map snd

    let playerBonus (players, closePartied) = 1 + players / 2 + closePartied / 2

    let calculateNoDrop (tc : TreasureClassNode) playersInput =
        let probSum = tc.SumNodesProbability() |> float
        let originalNoDrop = tc.NoDrop |> float
        let playerBonus = playerBonus (playersInput, 1) |> float
        let sum = originalNoDrop + probSum
        let pow =
            Math.Pow(
                (originalNoDrop / sum),
                playerBonus
            )
        int (
            (probSum) / (
                (1. / (pow)) - 1.
            )
        )

    let addItemMaps (baseMap : Map<ItemCode, float>) (m : Map<ItemCode, float>) =
        Map.fold (fun acc key value ->
            let v = 
                match Map.tryFind key acc with
                | Some x -> x + value
                | None -> value

            Map.add key v acc
        ) baseMap m

    type PickType =
    | Regular of pick : int
    /// Champion picks are (currently) negative in the json
    /// and indicate the chance should reset to avoid a nodrop
    | Champion of pick : int


    let calcChance x y c = ((float x) / (float y)) * c
    let calcItem picks chance = 1. - Math.Pow(1. - chance, float picks)

    let getPicks (tc) =
        if tc.Picks > 6 then 
            Regular 6
        elif tc.Picks < 0 then
            Champion (Math.Abs(tc.Picks))
        else
            Regular tc.Picks

    let mapItemCollection totalPicks tcChance (coll : ItemCollection) =
        let probSum = coll.SumProbability()
        coll.Items
        |> List.map (fun (item, prob) ->
            let chance =
                calcChance prob probSum tcChance
                |> calcItem totalPicks
            item.Code, chance
        )// |> Map.ofList
        |> List.groupBy fst
        |> List.map (fun (k, vs) ->
            match vs with
            | [] -> failwith "Unreachable state"
            | [x] -> x
            | x :: xs ->
                printfn "folding similar items within a collection"
                let folder (k1 : ItemCode, v1 : float) (_ : ItemCode, v2 : float) = k1, v1 + v2
                List.fold folder x xs
        ) |> Map.ofList


    type ShouldTraverse = 
    | Recurse of chance : float * next : TreasureClassNode
    | Exit of itemMap : Map<ItemCode, float>

    let mapTreasureClassNode (tryGetNode : string -> TreasureClassNode option) tcProbSum totalPicks tcChance node =
        let exitEmpty = Exit Map.empty
        match node with
        | ItemCollection (items, prob) ->
            /// we need to make sure we calculate the new chance at this level, too
            /// the colleciton itself is a drop & has a chance to be picked.
            let newChance = calcChance prob tcProbSum tcChance
            let itemMap = mapItemCollection totalPicks newChance items
            
            itemMap |> Exit
        | ItemCode (item, prob) ->
            let newChance = calcChance prob tcProbSum tcChance
            [ (item.Code, newChance) ] |> Map.ofList |> Exit
        | TreasureClassReference (name, _, prob) ->
            let newChance = calcChance prob tcProbSum tcChance
            let next = tryGetNode name
            match next with
            | Some next ->
                Recurse (newChance, next)
            | None ->
                printfn "Unable to find treasure class %s" name
                exitEmpty
        | UnknownTreasureClassReference (name, prob) ->
            let newChance = calcChance prob tcProbSum tcChance
            let next = tryGetNode name
            match next with
            | Some next ->
                Recurse (newChance, next)
            | None ->
                printfn "Unable to find unknown treasure class %s" name
                exitEmpty
        | RunesReference (name, prob) ->
            let newChance = calcChance prob tcProbSum tcChance
            let next = tryGetNode name
            match next with
            | Some next ->
                Recurse (newChance, next)
            | None ->
                printfn "Unable to find runes reference %s" name
                exitEmpty

    let traverseTreasureClasses treasureClasses tc = 
        let mutable chanceNoDrop = 0.
        let mutable chanceModifers : ChanceModifiers option = None
        let tryGetNode name = Map.tryFind name treasureClasses
        /// this is where modifiers will go, later
        /// unless i add those to the summaries?
        let rec traverse tc (tcChance : float) totalPicks (allItems : Map<ItemCode, float>) =
            printfn "In treasure class %s with chance %f and total picks %i. TC has picks %i" tc.Name tcChance totalPicks tc.Picks
            let picks = getPicks tc

            let sumProb = tc.SumAllProbability()

            let newNoDrop =
                let newNoDrop = calculateNoDrop tc 1
                printfn "New nodrop value is %A" newNoDrop
                newNoDrop

            chanceNoDrop <- chanceNoDrop + ((tcChance * (float newNoDrop)) / (float sumProb))
            chanceModifers <- addChances chanceModifers tc.Chances
            let sumProb = tc.SumNodesProbability() + newNoDrop
            match picks with
            | Regular pickValue ->
                // update the total picks
                let totalPicks = totalPicks * pickValue
                tc.Nodes
                |> List.map (fun node ->
                    match mapTreasureClassNode tryGetNode sumProb totalPicks tcChance node with
                    | Exit items -> items
                    | Recurse (newChance, next) ->
                        traverse next newChance totalPicks allItems
                )
                |> List.fold (fun mb ma -> addItemMaps mb ma) allItems
            | Champion picks -> 
                 /// Champions can exhaust their list of possible picks. Why? no idea.
                let mutable remainingPicks = picks
                printfn "In Champion picks with picks %i" picks
                tc.Nodes
                |> List.map (fun node ->
                    if remainingPicks > 0 then
                        let picked =
                            if node.Probability > remainingPicks then remainingPicks else node.Probability
                        remainingPicks <- remainingPicks - picked
                        match mapTreasureClassNode tryGetNode sumProb (picked * totalPicks) tcChance node with
                        | Exit items -> items
                        | Recurse (newChance, next) ->
                            traverse next newChance totalPicks allItems
                    else
                        Map.empty
                )
                |> List.fold (fun mb ma -> addItemMaps mb ma) allItems
            
        let items = traverse tc 1. 1 Map.empty
        items, chanceNoDrop, chanceModifers