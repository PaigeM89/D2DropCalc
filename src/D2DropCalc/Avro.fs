namespace D2DropCalc

module Avro =
  open Avro.FSharp

  let armorSchema = 
    match Schema.generate Schema.defaultOptions typeof<Types.Items.Armor> with
    | Ok schema -> schema
    | Error err -> failwithf "Schema generation error: %A" err

  let generateSchema<'a>() =
    match Schema.generate Schema.defaultOptions typeof<'a> with
    | Ok schema -> schema
    | Error err -> failwithf "Schema generation error: %A" err

  let armorSerializer : Types.Items.Armor -> System.IO.Stream -> unit =
    Serde.binarySerializer Serde.defaultSerializerOptions typeof<Types.Items.Armor> armorSchema
