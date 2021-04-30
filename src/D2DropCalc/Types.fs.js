import { toString, Union, Record } from "../D2DropCalc.SPA/src/.fable/fable-library.3.1.15/Types.js";
import { bool_type, list_type, tuple_type, union_type, record_type, option_type, int32_type, string_type } from "../D2DropCalc.SPA/src/.fable/fable-library.3.1.15/Reflection.js";
import { list as list_1, Auto_toString_5A41365E, option, object } from "../D2DropCalc.SPA/src/.fable/Thoth.Json.5.1.0/Encode.fs.js";
import { list as list_2, Auto_fromString_Z5CB6BD, int, string, object as object_1 } from "../D2DropCalc.SPA/src/.fable/Thoth.Json.5.1.0/Decode.fs.js";
import { equals, comparePrimitives, max as max_1, uncurry } from "../D2DropCalc.SPA/src/.fable/fable-library.3.1.15/Util.js";
import { interpolate, toText, printf, toFail } from "../D2DropCalc.SPA/src/.fable/fable-library.3.1.15/String.js";
import { value as value_17 } from "../D2DropCalc.SPA/src/.fable/fable-library.3.1.15/Option.js";
import { filter, sumBy, cons, map, ofArray } from "../D2DropCalc.SPA/src/.fable/fable-library.3.1.15/List.js";
import { FSharpResult$2 } from "../D2DropCalc.SPA/src/.fable/fable-library.3.1.15/Choice.js";

export class Items_UniqueItem extends Record {
    constructor(Name, BaseItemCode, BaseItemName, ItemLevel, ReqLevel, Rarity) {
        super();
        this.Name = Name;
        this.BaseItemCode = BaseItemCode;
        this.BaseItemName = BaseItemName;
        this.ItemLevel = ItemLevel;
        this.ReqLevel = ReqLevel;
        this.Rarity = Rarity;
    }
}

export function Items_UniqueItem$reflection() {
    return record_type("D2DropCalc.Types.Items.UniqueItem", [], Items_UniqueItem, () => [["Name", string_type], ["BaseItemCode", string_type], ["BaseItemName", string_type], ["ItemLevel", option_type(int32_type)], ["ReqLevel", option_type(int32_type)], ["Rarity", option_type(int32_type)]]);
}

export function Items_UniqueItem_Create(name, baseCode, baseName, ilvl, rlvl, rarity) {
    return new Items_UniqueItem(name, baseCode, baseName, ilvl, rlvl, rarity);
}

export function Items_UniqueItem__Encode(this$) {
    return object([["name", this$.Name], ["baseItemCode", this$.BaseItemCode], ["baseItemName", this$.BaseItemName], ["itemLevel", option((value_3) => value_3)(this$.ItemLevel)], ["reqLevel", option((value_5) => value_5)(this$.ReqLevel)], ["rarity", option((value_7) => value_7)(this$.Rarity)]]);
}

export function Items_UniqueItem_Decode() {
    return (path_3) => ((v) => object_1((get$) => (new Items_UniqueItem(get$.Required.Field("name", (path, value) => string(path, value)), get$.Required.Field("baseItemCode", (path_1, value_1) => string(path_1, value_1)), get$.Required.Field("baseItemName", (path_2, value_2) => string(path_2, value_2)), get$.Optional.Field("itemLevel", uncurry(2, int)), get$.Optional.Field("reqLevel", uncurry(2, int)), get$.Optional.Field("rarity", uncurry(2, int)))), path_3, v));
}

export class Items_BaseType extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Normal", "Exceptional", "Elite"];
    }
    toString() {
        const this$ = this;
        switch (this$.tag) {
            case 1: {
                return "Exceptional";
            }
            case 2: {
                return "Elite";
            }
            default: {
                return "Normal";
            }
        }
    }
}

export function Items_BaseType$reflection() {
    return union_type("D2DropCalc.Types.Items.BaseType", [], Items_BaseType, () => [[], [], []]);
}

export function Items_BaseType__MinimalString(this$) {
    switch (this$.tag) {
        case 1: {
            return "ex";
        }
        case 2: {
            return "el";
        }
        default: {
            return "no";
        }
    }
}

export function Items_BaseType__Encode(this$) {
    const str = (this$.tag === 1) ? "exceptional" : ((this$.tag === 2) ? "elite" : "normal");
    return str;
}

export function Items_BaseType_FromString_Z721C83C5(str) {
    switch (str) {
        case "normal": {
            return new Items_BaseType(0);
        }
        case "exceptional": {
            return new Items_BaseType(1);
        }
        case "elite": {
            return new Items_BaseType(2);
        }
        default: {
            return toFail(printf("Unknown base item type: \u0027%s\u0027"))(str);
        }
    }
}

export function Items_BaseType__EncodeMinimal(this$) {
    const str = Items_BaseType__MinimalString(this$);
    return str;
}

export function Items_BaseType_FromMinimalString_Z721C83C5(str) {
    switch (str) {
        case "no": {
            return new Items_BaseType(0);
        }
        case "ex": {
            return new Items_BaseType(1);
        }
        case "el": {
            return new Items_BaseType(2);
        }
        default: {
            return toFail(printf("Unknown base item type from minimal string: \u0027%s\u0027"))(str);
        }
    }
}

export class Items_Weapon extends Record {
    constructor(Name, Type, Code, Rarity, Speed, ItemLevel, ReqLevel, MaxSockets, ReqStrength, ReqDex, BaseType) {
        super();
        this.Name = Name;
        this.Type = Type;
        this.Code = Code;
        this.Rarity = Rarity;
        this.Speed = Speed;
        this.ItemLevel = ItemLevel;
        this.ReqLevel = ReqLevel;
        this.MaxSockets = MaxSockets;
        this.ReqStrength = ReqStrength;
        this.ReqDex = ReqDex;
        this.BaseType = BaseType;
    }
}

export function Items_Weapon$reflection() {
    return record_type("D2DropCalc.Types.Items.Weapon", [], Items_Weapon, () => [["Name", string_type], ["Type", string_type], ["Code", string_type], ["Rarity", option_type(int32_type)], ["Speed", option_type(int32_type)], ["ItemLevel", option_type(int32_type)], ["ReqLevel", option_type(int32_type)], ["MaxSockets", option_type(int32_type)], ["ReqStrength", option_type(int32_type)], ["ReqDex", option_type(int32_type)], ["BaseType", Items_BaseType$reflection()]]);
}

export function Items_Weapon_Create(name, _type, code, rarity, speed, ilvl, rlvl, sockets, str, dex, _base) {
    return new Items_Weapon(name, _type, code, rarity, speed, ilvl, rlvl, sockets, str, dex, _base);
}

export function Items_Weapon__Encode(this$) {
    return object([["name", this$.Name], ["type", this$.Type], ["code", this$.Code], ["rarity", option((value_3) => value_3)(this$.Rarity)], ["speed", option((value_5) => value_5)(this$.Speed)], ["itemLevel", option((value_7) => value_7)(this$.ItemLevel)], ["reqLevel", option((value_9) => value_9)(this$.ReqLevel)], ["maxSockets", option((value_11) => value_11)(this$.MaxSockets)], ["reqStrength", option((value_13) => value_13)(this$.ReqStrength)], ["reqDex", option((value_15) => value_15)(this$.ReqDex)], ["baseType", Items_BaseType__Encode(this$.BaseType)]]);
}

export function Items_Weapon__EncodeMinimal(this$) {
    return object([["n", this$.Name], ["t", this$.Type], ["c", this$.Code], ["r", option((value_3) => value_3)(this$.Rarity)], ["s", option((value_5) => value_5)(this$.Speed)], ["ilvl", option((value_7) => value_7)(this$.ItemLevel)], ["rlvl", option((value_9) => value_9)(this$.ReqLevel)], ["ms", option((value_11) => value_11)(this$.MaxSockets)], ["rs", option((value_13) => value_13)(this$.ReqStrength)], ["rd", option((value_15) => value_15)(this$.ReqDex)], ["bt", Items_BaseType__EncodeMinimal(this$.BaseType)]]);
}

export function Items_Weapon_Decoder() {
    return (path_4) => ((v) => object_1((get$) => (new Items_Weapon(get$.Required.Field("name", (path, value) => string(path, value)), get$.Required.Field("type", (path_1, value_1) => string(path_1, value_1)), get$.Required.Field("code", (path_2, value_2) => string(path_2, value_2)), get$.Optional.Field("rarity", uncurry(2, int)), get$.Optional.Field("speed", uncurry(2, int)), get$.Optional.Field("itemLevel", uncurry(2, int)), get$.Optional.Field("reqLevel", uncurry(2, int)), get$.Optional.Field("maxSockets", uncurry(2, int)), get$.Optional.Field("reqStrength", uncurry(2, int)), get$.Optional.Field("reqDex", uncurry(2, int)), Items_BaseType_FromString_Z721C83C5(get$.Required.Field("baseType", (path_3, value_3) => string(path_3, value_3))))), path_4, v));
}

export function Items_Weapon_DecoderMinimal() {
    return (path_4) => ((v) => object_1((get$) => (new Items_Weapon(get$.Required.Field("n", (path, value) => string(path, value)), get$.Required.Field("t", (path_1, value_1) => string(path_1, value_1)), get$.Required.Field("c", (path_2, value_2) => string(path_2, value_2)), get$.Optional.Field("r", uncurry(2, int)), get$.Optional.Field("s", uncurry(2, int)), get$.Optional.Field("ilvl", uncurry(2, int)), get$.Optional.Field("rlvl", uncurry(2, int)), get$.Optional.Field("ms", uncurry(2, int)), get$.Optional.Field("rs", uncurry(2, int)), get$.Optional.Field("rd", uncurry(2, int)), Items_BaseType_FromMinimalString_Z721C83C5(get$.Required.Field("bt", (path_3, value_3) => string(path_3, value_3))))), path_4, v));
}

export class Items_Armor extends Record {
    constructor(Name, Code, Rarity, ItemLevel, ReqLevel, ReqStrength, MaxSockets, SpeedPenalty, BaseType) {
        super();
        this.Name = Name;
        this.Code = Code;
        this.Rarity = Rarity;
        this.ItemLevel = ItemLevel;
        this.ReqLevel = ReqLevel;
        this.ReqStrength = ReqStrength;
        this.MaxSockets = MaxSockets;
        this.SpeedPenalty = SpeedPenalty;
        this.BaseType = BaseType;
    }
}

export function Items_Armor$reflection() {
    return record_type("D2DropCalc.Types.Items.Armor", [], Items_Armor, () => [["Name", string_type], ["Code", string_type], ["Rarity", option_type(int32_type)], ["ItemLevel", option_type(int32_type)], ["ReqLevel", option_type(int32_type)], ["ReqStrength", option_type(int32_type)], ["MaxSockets", option_type(int32_type)], ["SpeedPenalty", option_type(int32_type)], ["BaseType", Items_BaseType$reflection()]]);
}

export function Items_Armor_Create(name, code, rarity, ilvl, rlvl, str, sockets, penalty, _base) {
    return new Items_Armor(name, code, rarity, ilvl, rlvl, str, sockets, penalty, _base);
}

export function Items_Armor__Encode(this$) {
    return object([["name", this$.Name], ["code", this$.Code], ["rarity", option((value_2) => value_2)(this$.Rarity)], ["itemLevel", option((value_4) => value_4)(this$.ItemLevel)], ["reqLevel", option((value_6) => value_6)(this$.ReqLevel)], ["reqStrength", option((value_8) => value_8)(this$.ReqStrength)], ["sockets", option((value_10) => value_10)(this$.MaxSockets)], ["speedPenalty", option((value_12) => value_12)(this$.SpeedPenalty)], ["baseType", Items_BaseType__Encode(this$.BaseType)]]);
}

export function Items_Armor__EncodeMinimal(this$) {
    return object([["n", this$.Name], ["c", this$.Code], ["r", option((value_2) => value_2)(this$.Rarity)], ["ilvl", option((value_4) => value_4)(this$.ItemLevel)], ["rlvl", option((value_6) => value_6)(this$.ReqLevel)], ["rs", option((value_8) => value_8)(this$.ReqStrength)], ["so", option((value_10) => value_10)(this$.MaxSockets)], ["sp", option((value_12) => value_12)(this$.SpeedPenalty)], ["bt", Items_BaseType__EncodeMinimal(this$.BaseType)]]);
}

export function Items_Armor_Decoder() {
    return (path_3) => ((v) => object_1((get$) => (new Items_Armor(get$.Required.Field("name", (path, value) => string(path, value)), get$.Required.Field("code", (path_1, value_1) => string(path_1, value_1)), get$.Optional.Field("rarity", uncurry(2, int)), get$.Optional.Field("itemLevel", uncurry(2, int)), get$.Optional.Field("reqLevel", uncurry(2, int)), get$.Optional.Field("reqStrength", uncurry(2, int)), get$.Optional.Field("sockets", uncurry(2, int)), get$.Optional.Field("speedPenalty", uncurry(2, int)), Items_BaseType_FromString_Z721C83C5(get$.Required.Field("baseType", (path_2, value_2) => string(path_2, value_2))))), path_3, v));
}

export function Items_Armor_DecoderMinimal() {
    return (path_3) => ((v) => object_1((get$) => (new Items_Armor(get$.Required.Field("n", (path, value) => string(path, value)), get$.Required.Field("c", (path_1, value_1) => string(path_1, value_1)), get$.Optional.Field("r", uncurry(2, int)), get$.Optional.Field("ilvl", uncurry(2, int)), get$.Optional.Field("rlvl", uncurry(2, int)), get$.Optional.Field("rs", uncurry(2, int)), get$.Optional.Field("so", uncurry(2, int)), get$.Optional.Field("sp", uncurry(2, int)), Items_BaseType_FromMinimalString_Z721C83C5(get$.Required.Field("bt", (path_2, value_2) => string(path_2, value_2))))), path_3, v));
}

export function Items_encodeArmorAsData(armors) {
    const jstr = (str) => {
        Newtonsoft_Json_Linq_JValue_$ctor_Z721C83C5(str);
    };
    const jinto = (i) => {
        if (i != null) {
            Newtonsoft_Json_Linq_JValue_$ctor_4E60E31B(value_17(i));
        }
        else {
            Newtonsoft_Json_Linq_JValue_$ctor_Z721C83C5(null);
        }
    };
    const header = ofArray([jstr("name"), jstr("code"), jstr("rarity"), jstr("itemLevel"), jstr("reqLevl"), jstr("sockets"), jstr("speedPenalty"), jstr("baseType")]);
    const makeRow = (armor) => ofArray([jstr(armor.Name), jstr(armor.Code), jinto(armor.Rarity), jinto(armor.ItemLevel), jinto(armor.ReqLevel), jinto(armor.MaxSockets), jinto(armor.SpeedPenalty), jstr(Items_BaseType__MinimalString(armor.BaseType))]);
    const armorsEncoded = map(makeRow, armors);
    return cons(header, armorsEncoded);
}

export class ItemTree_JewelryType extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Ring", "Amulet", "Jewel", "GrandCharm", "LargeCharm", "SmallCharm"];
    }
}

export function ItemTree_JewelryType$reflection() {
    return union_type("D2DropCalc.Types.ItemTree.JewelryType", [], ItemTree_JewelryType, () => [[], [], [], [], [], []]);
}

export class ItemTree_ItemType extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Weapon", "Melee", "Bow", "Armor"];
    }
}

export function ItemTree_ItemType$reflection() {
    return union_type("D2DropCalc.Types.ItemTree.ItemType", [], ItemTree_ItemType, () => [[["code", string_type]], [["code", string_type]], [["code", string_type]], [["code", string_type]]]);
}

export function ItemTree_ItemType__get_Code(this$) {
    switch (this$.tag) {
        case 1: {
            const code_1 = this$.fields[0];
            return code_1;
        }
        case 2: {
            const code_2 = this$.fields[0];
            return code_2;
        }
        case 3: {
            const code_3 = this$.fields[0];
            return code_3;
        }
        default: {
            const code = this$.fields[0];
            return code;
        }
    }
}

export function ItemTree_isItemTypeForCode(code, itemType) {
    return code === ItemTree_ItemType__get_Code(itemType);
}

export class ItemTree_Item extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Base", "Rune", "Jewelry", "Keys", "Essence", "Gem", "Gold", "Potion", "Junk", "Ingredient", "Annihilus"];
    }
}

export function ItemTree_Item$reflection() {
    return union_type("D2DropCalc.Types.ItemTree.Item", [], ItemTree_Item, () => [[["itemType", ItemTree_ItemType$reflection()], ["level", int32_type]], [["code", string_type]], [["code", string_type], ["jewelryType", ItemTree_JewelryType$reflection()]], [["code", string_type]], [["code", string_type]], [["code", string_type]], [["multiplier", int32_type]], [["code", string_type]], [["code", string_type]], [["code", string_type]], [["code", string_type]]]);
}

export function ItemTree_Item__get_Code(this$) {
    switch (this$.tag) {
        case 1: {
            const code = this$.fields[0];
            return code;
        }
        case 2: {
            const code_1 = this$.fields[0];
            return code_1;
        }
        case 3: {
            const code_2 = this$.fields[0];
            return code_2;
        }
        case 4: {
            const code_3 = this$.fields[0];
            return code_3;
        }
        case 5: {
            const code_4 = this$.fields[0];
            return code_4;
        }
        case 6: {
            return "gld";
        }
        case 7: {
            const code_5 = this$.fields[0];
            return code_5;
        }
        case 8: {
            const code_6 = this$.fields[0];
            return code_6;
        }
        case 9: {
            const code_7 = this$.fields[0];
            return code_7;
        }
        case 10: {
            const code_8 = this$.fields[0];
            return code_8;
        }
        default: {
            const itemType = this$.fields[0];
            return ItemTree_ItemType__get_Code(itemType);
        }
    }
}

export function ItemTree_isItemForCode(code, item) {
    return ItemTree_Item__get_Code(item) === code;
}

export class ItemTree_ItemCollection extends Record {
    constructor(Name, Level, Code, Items) {
        super();
        this.Name = Name;
        this.Level = (Level | 0);
        this.Code = Code;
        this.Items = Items;
    }
}

export function ItemTree_ItemCollection$reflection() {
    return record_type("D2DropCalc.Types.ItemTree.ItemCollection", [], ItemTree_ItemCollection, () => [["Name", string_type], ["Level", int32_type], ["Code", string_type], ["Items", list_type(tuple_type(ItemTree_Item$reflection(), int32_type))]]);
}

export function ItemTree_ItemCollection__SumProbability(this$) {
    return sumBy((x) => x[1], this$.Items, {
        GetZero: () => 0,
        Add: (x_1, y) => (x_1 + y),
    });
}

export class ItemTree_Difficulty extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Normal", "Nightmare", "Hell"];
    }
    toString() {
        const this$ = this;
        switch (this$.tag) {
            case 1: {
                return "nightmare";
            }
            case 2: {
                return "hell";
            }
            default: {
                return "normal";
            }
        }
    }
}

export function ItemTree_Difficulty$reflection() {
    return union_type("D2DropCalc.Types.ItemTree.Difficulty", [], ItemTree_Difficulty, () => [[], [], []]);
}

export function ItemTree_Difficulty_FromString_Z721C83C5(str) {
    switch (str) {
        case "normal": {
            return new FSharpResult$2(0, new ItemTree_Difficulty(0));
        }
        case "nightmare": {
            return new FSharpResult$2(0, new ItemTree_Difficulty(1));
        }
        case "hell": {
            return new FSharpResult$2(0, new ItemTree_Difficulty(2));
        }
        default: {
            const e = str;
            return new FSharpResult$2(1, toText(interpolate("Could not create Difficulty from string \u0027%s%P()\u0027", [e])));
        }
    }
}

export class ItemTree_Act extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Act1", "Act2", "Act3", "Act4", "Act5"];
    }
}

export function ItemTree_Act$reflection() {
    return union_type("D2DropCalc.Types.ItemTree.Act", [], ItemTree_Act, () => [[], [], [], [], []]);
}

export class ItemTree_TreasureClassType extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Ammo", "Misc", "Gems", "Jewelry", "Potions", "CPot", "Junk", "Good", "Equipment", "Melee", "Bows", "Magic", "Boss", "CountessRune", "CountessItem", "UItem", "CItem"];
    }
}

export function ItemTree_TreasureClassType$reflection() {
    return union_type("D2DropCalc.Types.ItemTree.TreasureClassType", [], ItemTree_TreasureClassType, () => [[], [], [], [], [], [["act", ItemTree_Act$reflection()], ["diff", ItemTree_Difficulty$reflection()]], [["act", ItemTree_Act$reflection()], ["diff", ItemTree_Difficulty$reflection()]], [["act", ItemTree_Act$reflection()], ["diff", ItemTree_Difficulty$reflection()]], [["act", ItemTree_Act$reflection()], ["diff", ItemTree_Difficulty$reflection()]], [["act", ItemTree_Act$reflection()], ["diff", ItemTree_Difficulty$reflection()]], [["act", ItemTree_Act$reflection()], ["diff", ItemTree_Difficulty$reflection()]], [["act", ItemTree_Act$reflection()], ["diff", ItemTree_Difficulty$reflection()]], [["diff", ItemTree_Difficulty$reflection()], ["quest", bool_type]], [["diff", ItemTree_Difficulty$reflection()]], [["diff", ItemTree_Difficulty$reflection()]], [["act", ItemTree_Act$reflection()], ["diff", ItemTree_Difficulty$reflection()]], [["act", ItemTree_Act$reflection()], ["diff", ItemTree_Difficulty$reflection()]]]);
}

export function ItemTree_TreasureClassType__get_TypeName(this$) {
    switch (this$.tag) {
        case 1: {
            return "Misc";
        }
        case 2: {
            return "Gems";
        }
        case 3: {
            return "Jewelry";
        }
        case 4: {
            return "Potions";
        }
        case 5: {
            return "CPot";
        }
        case 6: {
            return "Junk";
        }
        case 7: {
            return "Good";
        }
        case 8: {
            return "Equipment";
        }
        case 9: {
            return "Melee";
        }
        case 10: {
            return "Bows";
        }
        case 11: {
            return "Magic";
        }
        case 12: {
            return "Boss";
        }
        case 13: {
            return "CountessRune";
        }
        case 14: {
            return "CountessItem";
        }
        case 15: {
            return "UItem";
        }
        case 16: {
            return "CItem";
        }
        default: {
            return "Ammo";
        }
    }
}

export class ItemTree_NodeValue extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["ItemCollection", "ItemCode", "TreasureClassReference", "UnknownTreasureClassReference", "RunesReference"];
    }
}

export function ItemTree_NodeValue$reflection() {
    return union_type("D2DropCalc.Types.ItemTree.NodeValue", [], ItemTree_NodeValue, () => [[["collection", ItemTree_ItemCollection$reflection()], ["probability", int32_type]], [["item", ItemTree_Item$reflection()], ["probability", int32_type]], [["name", string_type], ["tct", ItemTree_TreasureClassType$reflection()], ["probability", int32_type]], [["name", string_type], ["probability", int32_type]], [["runeCode", string_type], ["probability", int32_type]]]);
}

export function ItemTree_NodeValue__get_Probability(this$) {
    switch (this$.tag) {
        case 1: {
            const p_1 = this$.fields[1] | 0;
            return p_1 | 0;
        }
        case 2: {
            const p_2 = this$.fields[2] | 0;
            return p_2 | 0;
        }
        case 3: {
            const p_3 = this$.fields[1] | 0;
            return p_3 | 0;
        }
        case 4: {
            const p_4 = this$.fields[1] | 0;
            return p_4 | 0;
        }
        default: {
            const p = this$.fields[1] | 0;
            return p | 0;
        }
    }
}

export function ItemTree_NodeValue__get_Level(this$) {
    if (this$.tag === 0) {
        const coll = this$.fields[0];
        return coll.Level;
    }
    else {
        return void 0;
    }
}

export class ItemTree_ChanceModifiers extends Record {
    constructor(Unique, Set$, Rare, Magic) {
        super();
        this.Unique = Unique;
        this.Set = Set$;
        this.Rare = Rare;
        this.Magic = Magic;
    }
}

export function ItemTree_ChanceModifiers$reflection() {
    return record_type("D2DropCalc.Types.ItemTree.ChanceModifiers", [], ItemTree_ChanceModifiers, () => [["Unique", option_type(int32_type)], ["Set", option_type(int32_type)], ["Rare", option_type(int32_type)], ["Magic", option_type(int32_type)]]);
}

export function ItemTree_ChanceModifiers_get_Empty() {
    return new ItemTree_ChanceModifiers(void 0, void 0, void 0, void 0);
}

export function ItemTree_addChances(cm1, cm2) {
    const matchValue = [cm1, cm2];
    if (matchValue[0] == null) {
        if (matchValue[1] == null) {
            return void 0;
        }
        else {
            const cm2_2 = matchValue[1];
            return cm2_2;
        }
    }
    else if (matchValue[1] == null) {
        const cm1_2 = matchValue[0];
        return cm1_2;
    }
    else {
        const cm1_1 = matchValue[0];
        const cm2_1 = matchValue[1];
        const max = (io1, io2) => {
            const matchValue_1 = [io1, io2];
            if (matchValue_1[0] == null) {
                if (matchValue_1[1] == null) {
                    return void 0;
                }
                else {
                    const y_2 = matchValue_1[1] | 0;
                    return y_2;
                }
            }
            else if (matchValue_1[1] == null) {
                const x_2 = matchValue_1[0] | 0;
                return x_2;
            }
            else {
                const x = matchValue_1[0] | 0;
                const y = matchValue_1[1] | 0;
                return max_1((x_1, y_1) => comparePrimitives(x_1, y_1), x, y);
            }
        };
        return new ItemTree_ChanceModifiers(max(cm1_1.Unique, cm2_1.Unique), max(cm1_1.Set, cm2_1.Set), max(cm1_1.Rare, cm2_1.Rare), max(cm1_1.Magic, cm2_1.Magic));
    }
}

export class ItemTree_TreasureClassNode extends Record {
    constructor(Name, Nodes, Picks, NoDrop, TreasureClassType, Chances) {
        super();
        this.Name = Name;
        this.Nodes = Nodes;
        this.Picks = (Picks | 0);
        this.NoDrop = (NoDrop | 0);
        this.TreasureClassType = TreasureClassType;
        this.Chances = Chances;
    }
}

export function ItemTree_TreasureClassNode$reflection() {
    return record_type("D2DropCalc.Types.ItemTree.TreasureClassNode", [], ItemTree_TreasureClassNode, () => [["Name", string_type], ["Nodes", list_type(ItemTree_NodeValue$reflection())], ["Picks", int32_type], ["NoDrop", int32_type], ["TreasureClassType", option_type(ItemTree_TreasureClassType$reflection())], ["Chances", option_type(ItemTree_ChanceModifiers$reflection())]]);
}

export function ItemTree_TreasureClassNode__SumNodesProbability(this$) {
    return sumBy((x) => ItemTree_NodeValue__get_Probability(x), this$.Nodes, {
        GetZero: () => 0,
        Add: (x_1, y) => (x_1 + y),
    });
}

export function ItemTree_TreasureClassNode__SumAllProbability(this$) {
    return ItemTree_TreasureClassNode__SumNodesProbability(this$) + this$.NoDrop;
}

export function ItemTree_TreasureClassNode__Encode(this$) {
    return Auto_toString_5A41365E(0, this$, void 0, void 0, void 0, {
        ResolveType: ItemTree_TreasureClassNode$reflection,
    });
}

export function ItemTree_TreasureClassNode_Decode_Z721C83C5(str) {
    return Auto_fromString_Z5CB6BD(str, void 0, void 0, {
        ResolveType: ItemTree_TreasureClassNode$reflection,
    });
}

export class Monsters_MonsterDropQuality extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Normal", "Champion", "Unique", "Quest"];
    }
    toString() {
        const this$ = this;
        switch (this$.tag) {
            case 1: {
                return "champion";
            }
            case 2: {
                return "unique";
            }
            case 3: {
                return "quest";
            }
            default: {
                return "normal";
            }
        }
    }
}

export function Monsters_MonsterDropQuality$reflection() {
    return union_type("D2DropCalc.Types.Monsters.MonsterDropQuality", [], Monsters_MonsterDropQuality, () => [[], [], [], []]);
}

export function Monsters_MonsterDropQuality_FromString_Z721C83C5(str) {
    switch (str) {
        case "normal": {
            return new FSharpResult$2(0, new Monsters_MonsterDropQuality(0));
        }
        case "champion": {
            return new FSharpResult$2(0, new Monsters_MonsterDropQuality(1));
        }
        case "unique": {
            return new FSharpResult$2(0, new Monsters_MonsterDropQuality(2));
        }
        case "quest": {
            return new FSharpResult$2(0, new Monsters_MonsterDropQuality(3));
        }
        default: {
            const e = str;
            return new FSharpResult$2(1, toText(interpolate("Could not create monster drop quality from string \u0027%s%P()\u0027", [e])));
        }
    }
}

export class Monsters_Entrypoint extends Record {
    constructor(Quality, Diff, ItemTreeNode) {
        super();
        this.Quality = Quality;
        this.Diff = Diff;
        this.ItemTreeNode = ItemTreeNode;
    }
}

export function Monsters_Entrypoint$reflection() {
    return record_type("D2DropCalc.Types.Monsters.Entrypoint", [], Monsters_Entrypoint, () => [["Quality", Monsters_MonsterDropQuality$reflection()], ["Diff", ItemTree_Difficulty$reflection()], ["ItemTreeNode", string_type]]);
}

export function Monsters_Entrypoint__Encode(this$) {
    return object([["quality", toString(this$.Quality)], ["difficulty", toString(this$.Diff)], ["node", this$.ItemTreeNode]]);
}

export function Monsters_Entrypoint_Decoder() {
    return (path_3) => ((v) => object_1((get$) => {
        const qualRes = Monsters_MonsterDropQuality_FromString_Z721C83C5(get$.Required.Field("quality", (path, value) => string(path, value)));
        const diffRes = ItemTree_Difficulty_FromString_Z721C83C5(get$.Required.Field("difficulty", (path_1, value_1) => string(path_1, value_1)));
        const matchValue = [qualRes, diffRes];
        let pattern_matching_result, diff, qual, e;
        const copyOfStruct = matchValue[0];
        if (copyOfStruct.tag === 1) {
            pattern_matching_result = 1;
            e = copyOfStruct.fields[0];
        }
        else {
            const copyOfStruct_1 = matchValue[1];
            if (copyOfStruct_1.tag === 1) {
                pattern_matching_result = 1;
                e = copyOfStruct_1.fields[0];
            }
            else {
                pattern_matching_result = 0;
                diff = copyOfStruct_1.fields[0];
                qual = copyOfStruct.fields[0];
            }
        }
        switch (pattern_matching_result) {
            case 0: {
                return new Monsters_Entrypoint(qual, diff, get$.Required.Field("node", (path_2, value_2) => string(path_2, value_2)));
            }
            case 1: {
                throw (new Error(toText(interpolate("Unable to decode entrypoint : \u0027%s%P()\u0027", [e]))));
            }
        }
    }, path_3, v));
}

export class Monsters_Monster extends Record {
    constructor(Id, Name, Level, LevelNightmare, LevelHell, ItemTreeEntrypoints) {
        super();
        this.Id = Id;
        this.Name = Name;
        this.Level = (Level | 0);
        this.LevelNightmare = LevelNightmare;
        this.LevelHell = LevelHell;
        this.ItemTreeEntrypoints = ItemTreeEntrypoints;
    }
}

export function Monsters_Monster$reflection() {
    return record_type("D2DropCalc.Types.Monsters.Monster", [], Monsters_Monster, () => [["Id", string_type], ["Name", string_type], ["Level", int32_type], ["LevelNightmare", option_type(int32_type)], ["LevelHell", option_type(int32_type)], ["ItemTreeEntrypoints", list_type(Monsters_Entrypoint$reflection())]]);
}

export function Monsters_Monster__Encode(this$) {
    return object([["id", this$.Id], ["name", this$.Name], ["level", this$.Level], ["level(N)", option((value_3) => value_3)(this$.LevelNightmare)], ["level(H)", option((value_5) => value_5)(this$.LevelHell)], ["entrypoints", list_1(map((x) => Monsters_Entrypoint__Encode(x), this$.ItemTreeEntrypoints))]]);
}

export function Monsters_Monster_Decoder() {
    return (path_3) => ((v) => object_1((get$) => {
        let decoder;
        const entrypoints = get$.Required.Field("entrypoints", uncurry(2, (decoder = Monsters_Entrypoint_Decoder(), (path) => ((value) => list_2(uncurry(2, decoder), path, value)))));
        return new Monsters_Monster(get$.Required.Field("id", (path_1, value_1) => string(path_1, value_1)), get$.Required.Field("name", (path_2, value_2) => string(path_2, value_2)), get$.Required.Field("level", uncurry(2, int)), get$.Optional.Field("level(N)", uncurry(2, int)), get$.Optional.Field("level(H)", uncurry(2, int)), entrypoints);
    }, path_3, v));
}

export function Monsters_getEntrypointsForDifficulty(diff, eps) {
    return filter((x) => equals(x[1], diff), eps);
}

export function Monsters_getEntrypointsForQuality(qual, eps) {
    return filter((x) => equals(x[0], qual), eps);
}

