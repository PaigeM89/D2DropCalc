namespace D2DropCalc.SPA

module Fetch =
  open Fable.SimpleHttp
  open D2DropCalc.Types
  open D2DropCalc.Types.Items

  #if FABLE_COMPILER
  open Thoth.Json
  #else
  open Thoth.Json.Net
  #endif

  type FetchError =
  | NotFound
  | Unknown of code : int * response : string
  | DecodeError of error : string

  let private decodeArmors (armors : string) =
    let decoder = Armor.DecoderMinimal()
    let decodeResult = Decode.fromString (Thoth.Json.Decode.list decoder) armors
    match decodeResult with
    | Ok values -> Ok values
    | Error e -> DecodeError e |> Error
  
  let private decodeWeapons (weapons : string)=
    let decoder = Weapon.DecoderMinimal()
    let decodeResult = Decode.fromString (Thoth.Json.Decode.list decoder) weapons
    match decodeResult with
    | Ok values -> Ok values
    | Error e -> DecodeError e |> Error

  let private decodeMonsters (monsters : string) =
    let decoder = Monsters.Monster.Decoder()
    let decodeResult = Decode.fromString (Thoth.Json.Decode.list decoder) monsters
    match decodeResult with
    | Ok values -> Ok values
    | Error e -> DecodeError e |> Error

  let private genericFetch route decoder =
    async {
      let! (statusCode, responseText) = Http.get route
      match statusCode with
      | 200 ->
        printfn $"successfully hit %s{route} endpoint"
        return responseText |> decoder
      | 404 ->
        printfn "route '%s' not found, response text is '%s'" route responseText
        return (Error NotFound)
      | x ->
        printfn "unknown error fetching from route '%s'. Code is %i. Response text is '%s'" route x responseText
        return (Error (Unknown (x, "Unknown error")))
    }

  let getArmors() =
    let route = "/api/armors"
    genericFetch route decodeArmors

  let getWeapons() = genericFetch "/api/weapons" decodeWeapons

  let getMonsters() =
    printfn "Fetching monsters"
    genericFetch "/api/monsters" decodeMonsters