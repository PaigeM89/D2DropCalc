import { Union } from "./.fable/fable-library.3.1.15/Types.js";
import { union_type, string_type, int32_type } from "./.fable/fable-library.3.1.15/Reflection.js";
import { Items_Weapon_DecoderMinimal, Items_Armor_DecoderMinimal } from "../../D2DropCalc/Types.fs.js";
import { list, fromString } from "./.fable/Thoth.Json.5.1.0/Decode.fs.js";
import { uncurry } from "./.fable/fable-library.3.1.15/Util.js";
import { FSharpResult$2 } from "./.fable/fable-library.3.1.15/Choice.js";
import { singleton } from "./.fable/fable-library.3.1.15/AsyncBuilder.js";
import { Http_get } from "./.fable/Fable.SimpleHttp.3.0.0/Http.fs.js";
import { printf, toConsole } from "./.fable/fable-library.3.1.15/String.js";

export class FetchError extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["NotFound", "Unknown", "DecodeError"];
    }
}

export function FetchError$reflection() {
    return union_type("D2DropCalc.SPA.Fetch.FetchError", [], FetchError, () => [[], [["code", int32_type], ["response", string_type]], [["error", string_type]]]);
}

function decodeArmors(armors) {
    const decoder = Items_Armor_DecoderMinimal();
    const decodeResult = fromString((path, value) => list(uncurry(2, decoder), path, value), armors);
    if (decodeResult.tag === 1) {
        const e = decodeResult.fields[0];
        return new FSharpResult$2(1, new FetchError(2, e));
    }
    else {
        const values = decodeResult.fields[0];
        return new FSharpResult$2(0, values);
    }
}

function decodeWeapons(weapons) {
    const decoder = Items_Weapon_DecoderMinimal();
    const decodeResult = fromString((path, value) => list(uncurry(2, decoder), path, value), weapons);
    if (decodeResult.tag === 1) {
        const e = decodeResult.fields[0];
        return new FSharpResult$2(1, new FetchError(2, e));
    }
    else {
        const values = decodeResult.fields[0];
        return new FSharpResult$2(0, values);
    }
}

function genericFetch(route, decoder) {
    return singleton.Delay(() => singleton.Bind(Http_get(route), (_arg1) => {
        const statusCode = _arg1[0] | 0;
        const responseText = _arg1[1];
        switch (statusCode) {
            case 200: {
                toConsole(printf("successfully hit armors endpoint"));
                return singleton.Return(decoder(responseText));
            }
            case 404: {
                toConsole(printf("route \u0027%s\u0027 not found, response text is \u0027%s\u0027"))(route)(responseText);
                return singleton.Return(new FSharpResult$2(1, new FetchError(0)));
            }
            default: {
                const x = statusCode | 0;
                toConsole(printf("unknown error fetching from route \u0027%s\u0027. Code is %i. Response text is \u0027%s\u0027"))(route)(x)(responseText);
                return singleton.Return(new FSharpResult$2(1, new FetchError(1, x, "Unknown error")));
            }
        }
    }));
}

export function getArmors() {
    const route = "/api/armors";
    return genericFetch(route, (armors) => decodeArmors(armors));
}

export function getWeapons() {
    return genericFetch("/api/weapons", (weapons) => decodeWeapons(weapons));
}

