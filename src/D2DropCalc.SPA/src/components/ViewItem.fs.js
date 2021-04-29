import { createElement } from "react";
import * as react from "react";
import { label } from "../.fable/Fulma.2.10.0/Elements/Form/Label.fs.js";
import { ofArray, map, append, singleton, empty } from "../.fable/fable-library.3.1.15/List.js";
import { Record } from "../.fable/fable-library.3.1.15/Types.js";
import { Items_Armor$reflection, Items_Weapon$reflection } from "../../../D2DropCalc/Types.fs.js";
import { record_type, list_type } from "../.fable/fable-library.3.1.15/Reflection.js";
import { SearchableWithFunc, SearchableProps, createDDLValue } from "./micro/Dropdown.fs.js";
import { useReact_useEffect_Z101E1A95, useFeliz_React__React_useState_Static_1505, React_functionComponent_2F9D7239 } from "../.fable/Feliz.1.43.0/React.fs.js";
import { equals } from "../.fable/fable-library.3.1.15/Util.js";
import { interpolate, toText, printf, toConsole } from "../.fable/fable-library.3.1.15/String.js";
import { singleton as singleton_1, append as append_1, delay, toList } from "../.fable/fable-library.3.1.15/Seq.js";
import { columns } from "../.fable/Fulma.2.10.0/Layouts/Columns.fs.js";
import { Option, ISize, column } from "../.fable/Fulma.2.10.0/Layouts/Column.fs.js";
import { Screen } from "../.fable/Fulma.2.10.0/Common.fs.js";
import { useFeliz_React__React_useDeferredCallback_Static_7088D81D, Deferred$1 } from "../.fable/Feliz.UseDeferred.1.4.1/UseDeferred.fs.js";
import { getWeapons, getArmors } from "../Fetch.fs.js";
import { ErrorMessage } from "./micro/Error.fs.js";
import { Fa_IconOption, Fa_i } from "../.fable/Fable.FontAwesome.2.0.0/FontAwesome.fs.js";

function ItemTemplates_ViewArmor(armor) {
    return react.createElement("div", {}, label(empty(), singleton("Armor stuff goes here")));
}

export class SearchProps extends Record {
    constructor(weapons, armors) {
        super();
        this.weapons = weapons;
        this.armors = armors;
    }
}

export function SearchProps$reflection() {
    return record_type("D2DropCalc.SPA.Components.ViewItem.SearchProps", [], SearchProps, () => [["weapons", list_type(Items_Weapon$reflection())], ["armors", list_type(Items_Armor$reflection())]]);
}

export function armorDDLValue(armor) {
    return createDDLValue(armor.Code, armor.Name);
}

export function weaponDDLValue(weap) {
    return createDDLValue(weap.Code, weap.Name);
}

const renderSearch = React_functionComponent_2F9D7239((props) => {
    const patternInput = useFeliz_React__React_useState_Static_1505(void 0);
    const setSelected = patternInput[1];
    const selectValue = patternInput[0];
    const patternInput_1 = useFeliz_React__React_useState_Static_1505(empty());
    const setDDLValues = patternInput_1[1];
    const ddlValues = patternInput_1[0];
    if (equals(ddlValues, empty())) {
        const values = append(map((armor) => armorDDLValue(armor), props.armors), map((weap) => weaponDDLValue(weap), props.weapons));
        setDDLValues(values);
    }
    const onChange = (value) => {
        toConsole(printf("on change function fired, value is %s"))(value);
        setSelected(value);
    };
    const props_1 = new SearchableProps(ddlValues, "Select an item base", onChange);
    const searchable = SearchableWithFunc(props_1);
    return react.createElement("div", {}, ...toList(delay(() => append_1(singleton_1(columns(empty(), singleton(column(singleton(new Option(0, new Screen(0), new ISize(6))), singleton(searchable))))), delay(() => {
        if (selectValue == null) {
            return singleton_1(null);
        }
        else {
            const sv = selectValue;
            return singleton_1(label(empty(), singleton(toText(interpolate("You selected: \u0027%A%P()\u0027", [sv])))));
        }
    })))));
});

export function SelectAndViewItem() {
    const patternInput = useFeliz_React__React_useState_Static_1505(new Deferred$1(0));
    const setArmors = patternInput[1];
    const armors = patternInput[0];
    const patternInput_1 = useFeliz_React__React_useState_Static_1505(new Deferred$1(0));
    const weapons = patternInput_1[0];
    const setWeapons = patternInput_1[1];
    const startLoadingArmors = useFeliz_React__React_useDeferredCallback_Static_7088D81D(getArmors, setArmors);
    const startLoadingWeapons = useFeliz_React__React_useDeferredCallback_Static_7088D81D(getWeapons, setWeapons);
    useReact_useEffect_Z101E1A95(startLoadingArmors, []);
    useReact_useEffect_Z101E1A95(startLoadingWeapons, []);
    const matchValue = [armors, weapons];
    let pattern_matching_result, armors_1, weapons_1, e, e_1, error, error_1;
    if (matchValue[0].tag === 2) {
        if (matchValue[0].fields[0].tag === 1) {
            if (matchValue[1].tag === 2) {
                pattern_matching_result = 1;
                e = matchValue[0].fields[0].fields[0];
            }
            else if (matchValue[1].tag === 3) {
                pattern_matching_result = 4;
                error_1 = matchValue[1].fields[0];
            }
            else {
                pattern_matching_result = 5;
            }
        }
        else if (matchValue[1].tag === 2) {
            if (matchValue[1].fields[0].tag === 1) {
                pattern_matching_result = 2;
                e_1 = matchValue[1].fields[0].fields[0];
            }
            else {
                pattern_matching_result = 0;
                armors_1 = matchValue[0].fields[0].fields[0];
                weapons_1 = matchValue[1].fields[0].fields[0];
            }
        }
        else if (matchValue[1].tag === 3) {
            pattern_matching_result = 4;
            error_1 = matchValue[1].fields[0];
        }
        else {
            pattern_matching_result = 5;
        }
    }
    else if (matchValue[0].tag === 3) {
        pattern_matching_result = 3;
        error = matchValue[0].fields[0];
    }
    else if (matchValue[1].tag === 3) {
        pattern_matching_result = 4;
        error_1 = matchValue[1].fields[0];
    }
    else {
        pattern_matching_result = 5;
    }
    switch (pattern_matching_result) {
        case 0: {
            const props = new SearchProps(weapons_1, armors_1);
            return renderSearch(props);
        }
        case 1: {
            toConsole(printf("error loading armors: %A"))(e);
            return createElement(ErrorMessage, {
                msg: "Unable to load data",
            });
        }
        case 2: {
            toConsole(printf("error loading weapons: %A"))(e_1);
            return createElement(ErrorMessage, {
                msg: "Unable to load data",
            });
        }
        case 3: {
            toConsole(printf("deffered error: %A"))(error);
            return createElement(ErrorMessage, {
                msg: "Unable to resolve data load",
            });
        }
        case 4: {
            toConsole(printf("deffered error: %A"))(error_1);
            return createElement(ErrorMessage, {
                msg: "Unable to resolve data load",
            });
        }
        case 5: {
            return react.createElement("div", {
                className: "block fa-3x",
            }, Fa_i(ofArray([new Fa_IconOption(11, "fas fa-spinner"), new Fa_IconOption(12)]), []));
        }
    }
}

