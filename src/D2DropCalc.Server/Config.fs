namespace D2DropCalc.Server

module Config =

    type Config = {
        JsonDir : string
    } with
        static member Create path = {
            JsonDir = path
        }

        static member Default() = {
            JsonDir = ""
        }

    type IServeConfig =
        abstract member Serve: unit -> Config

    type ILoadConfig =
        abstract member Load: unit -> Config