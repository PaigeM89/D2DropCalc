namespace D2DropCalc.Server

module TreasureClasses =
    open D2DropCalc.TreeTraversal

    module Loading =
        open System
        open System.IO

        [<Literal>]
        let Armors = "armors.json"

        // let loadArmors (conf : Config.Config) =
        //     let dir = conf.JsonDir
        //     let files = IO.Directory.GetFiles dir
        //     printfn "All files are %A" files
        //     files
        //     |> Array.find (fun x -> x.Contains Armors)
        //     |> D2DropCalc.Types.Loading.loadArmorsFromPath


    let calcPlayerBonus (players, closePartied) = playerBonus (players, closePartied)

