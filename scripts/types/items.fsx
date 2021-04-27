#r "nuget: Thoth.Json.Net"
open Thoth.Json.Net

type UniqueItem = {
    Name : string
    BaseItemCode : string
    BaseItemName : string
    ItemLevel : int option
    ReqLevel : int option
    Rarity : int option
} with
    static member Create name baseCode baseName ilvl rlvl rarity = {
        Name = name
        BaseItemCode = baseCode
        BaseItemName = baseName
        ItemLevel = ilvl
        ReqLevel = rlvl
        Rarity = rarity
    }

    member this.Encode() =
        Encode.object [
            "name", Encode.string this.Name
            "baseItemCode", Encode.string this.BaseItemCode
            "baseItemName", Encode.string this.BaseItemName
            "itemLevel", Encode.option Encode.int this.ItemLevel
            "reqLevel", Encode.option Encode.int this.ReqLevel
            "rarity", Encode.option Encode.int this.Rarity
        ]

type Weapon = {
    Name : string
    Type : string
    Code : string
    Rarity : int option
    Speed : int option
    ItemLevel : int option
    ReqLevel : int option
    MaxSockets : int option
    ReqStrength : int option
    ReqDex : int option
} with
    static member Create name _type code rarity speed ilvl rlvl sockets str dex = {
        Name = name
        Type = _type
        Code = code
        Rarity = rarity
        Speed = speed
        ItemLevel = ilvl
        ReqLevel = rlvl
        MaxSockets = sockets
        ReqStrength = str
        ReqDex = dex
    }

    member this.Encode() =
        Encode.object [
            "name", Encode.string this.Name
            "type", Encode.string this.Type
            "code", Encode.string this.Code
            "rarity", Encode.option Encode.int this.Rarity
            "speed", Encode.option Encode.int this.Speed
            "itemLevel", Encode.option Encode.int this.ItemLevel
            "reqLevel", Encode.option Encode.int this.ReqLevel
            "maxSockets", Encode.option Encode.int this.MaxSockets
            "reqStrength",  Encode.option Encode.int this.ReqStrength
            "reqDex",  Encode.option Encode.int this.ReqDex
        ]

type Armor = {
    
}
