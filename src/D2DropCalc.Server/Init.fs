namespace D2DropCalc.Server

module LoadJson =
    open Config
    open System.IO

    let private loadAppConfigMap() =
        let lines = File.ReadAllLines "appconfig.conf"
        lines
        |> Seq.fold (fun map line ->
            let split = 
                match line.IndexOf(':') with
                | -1 | 0 -> None
                | n -> Some (line.Substring(0, n).Trim(), line.Substring(n+1).Trim())
            match split with
            | Some (k, v) -> map |> Map.add k v
            | None -> map
        ) Map.empty

    let readAppConfig() =
        let conf = loadAppConfigMap()
        match Map.tryFind "jsonDirPath" conf with
        | Some v -> Config.Create v
        | None -> Config.Default()

    type ConfigServer() =
        let mutable _config : Config = Config.Default()

        interface IServeConfig with
            member this.Serve() = _config
        interface ILoadConfig with
            member this.Load() =
                _config <- readAppConfig()
                _config