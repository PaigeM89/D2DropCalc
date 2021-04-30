import { useReact_useEffect_Z101E1A95, useFeliz_React__React_useState_Static_1505, React_functionComponent_2F9D7239 } from "../.fable/Feliz.1.43.0/React.fs.js";
import { interpolate, toText, printf, toConsole, join } from "../.fable/fable-library.3.1.15/String.js";
import { ofArray, isEmpty, tryFind, singleton as singleton_1, empty, map } from "../.fable/fable-library.3.1.15/List.js";
import { int32ToString } from "../.fable/fable-library.3.1.15/Util.js";
import { createElement } from "react";
import * as react from "react";
import { empty as empty_1, singleton, append, delay, toList } from "../.fable/fable-library.3.1.15/Seq.js";
import { label } from "../.fable/Fulma.2.10.0/Elements/Form/Label.fs.js";
import { value } from "../.fable/fable-library.3.1.15/Option.js";
import { Record } from "../.fable/fable-library.3.1.15/Types.js";
import { record_type, lambda_type, unit_type, string_type } from "../.fable/fable-library.3.1.15/Reflection.js";
import { SearchableWithFunc, SearchableProps, createDDLValue } from "./micro/Dropdown.fs.js";
import { useFeliz_React__React_useDeferredCallback_Static_7088D81D, Deferred$1 } from "../.fable/Feliz.UseDeferred.1.4.1/UseDeferred.fs.js";
import { getMonsters } from "../Fetch.fs.js";
import { ErrorMessage } from "./micro/Error.fs.js";
import { Fa_IconOption, Fa_i } from "../.fable/Fable.FontAwesome.2.0.0/FontAwesome.fs.js";

export const viewMonsterDetails = React_functionComponent_2F9D7239((monster) => {
    let treasureClassNames;
    const str = join(", ", map((ep) => ep.ItemTreeNode, monster.ItemTreeEntrypoints));
    treasureClassNames = ((str === "") ? "None" : str);
    const intStrOrNone = (io) => {
        if (io == null) {
            return "None";
        }
        else {
            const x = io | 0;
            return int32ToString(x);
        }
    };
    return react.createElement("div", {}, ...toList(delay(() => append(singleton(label(empty(), singleton_1(monster.Name))), delay(() => append(singleton(label(empty(), singleton_1("Level: " + int32ToString(monster.Level)))), delay(() => append((monster.LevelNightmare != null) ? singleton(label(empty(), singleton_1("Level (N): " + int32ToString(value(monster.LevelNightmare))))) : empty_1(), delay(() => append((monster.LevelHell != null) ? singleton(label(empty(), singleton_1("Level (N): " + int32ToString(value(monster.LevelHell))))) : empty_1(), delay(() => singleton(label(empty(), singleton_1("Treasure Classes: " + treasureClassNames))))))))))))));
});

export class SearchDropdownProps extends Record {
    constructor(callback) {
        super();
        this.callback = callback;
    }
}

export function SearchDropdownProps$reflection() {
    return record_type("D2DropCalc.SPA.Components.MonsterDrop.SearchDropdownProps", [], SearchDropdownProps, () => [["callback", lambda_type(string_type, unit_type)]]);
}

export function monstersToDDL(monsters) {
    toConsole(printf("creating monster ddl list"));
    return map((m) => createDDLValue(m.Id, m.Name), monsters);
}

export function monsterDDLProps(callback, values) {
    return new SearchableProps(values, "Select a monster", callback);
}

export const monsterSearchDropdown = React_functionComponent_2F9D7239((props) => {
    toConsole(printf("in monsters search dropdown"));
    const patternInput = useFeliz_React__React_useState_Static_1505(new Deferred$1(0));
    const setMonsters = patternInput[1];
    const monsters = patternInput[0];
    const patternInput_1 = useFeliz_React__React_useState_Static_1505(void 0);
    const setSelectedMonster = patternInput_1[1];
    const selectedMonster = patternInput_1[0];
    const patternInput_2 = useFeliz_React__React_useState_Static_1505(void 0);
    const setSelected = patternInput_2[1];
    const selected = patternInput_2[0];
    const patternInput_3 = useFeliz_React__React_useState_Static_1505(empty());
    const setDDLValues = patternInput_3[1];
    const ddlValues = patternInput_3[0];
    const startLoadingMonsters = useFeliz_React__React_useDeferredCallback_Static_7088D81D(getMonsters, setMonsters);
    useReact_useEffect_Z101E1A95(startLoadingMonsters, []);
    const setMonster = (monsters_1, id) => {
        toConsole(printf("trying to set selected monster for id %s"))(id);
        const matchValue = tryFind((x) => (x.Id === id), monsters_1);
        if (matchValue == null) {
        }
        else {
            const v = matchValue;
            setSelectedMonster(v);
        }
    };
    const selectionChanged = (id_1) => {
        let x_1;
        toConsole(printf("in selection change callback being invoked, id is %s"))(id_1);
        let pattern_matching_result;
        if (selected != null) {
            if (x_1 = selected, x_1 === id_1) {
                pattern_matching_result = 0;
            }
            else {
                pattern_matching_result = 1;
            }
        }
        else {
            pattern_matching_result = 1;
        }
        switch (pattern_matching_result) {
            case 0: {
                break;
            }
            case 1: {
                setSelected(id_1);
                props.callback(id_1);
                break;
            }
        }
    };
    if (monsters.tag === 2) {
        if (monsters.fields[0].tag === 1) {
            const e = monsters.fields[0].fields[0];
            toConsole(printf("in error handler for resolved deferred"));
            return createElement(ErrorMessage, {
                msg: "There was an error fetching or decoding monster data",
            });
        }
        else {
            const monsters_2 = monsters.fields[0].fields[0];
            toConsole(printf("deferred resolved ok"));
            let ddlValues_1;
            if (isEmpty(ddlValues)) {
                toConsole(printf("ddlvalues are empty, populating"));
                const values = monstersToDDL(monsters_2);
                setDDLValues(values);
                toConsole(printf("returning list of ddl values"));
                ddlValues_1 = values;
            }
            else {
                toConsole(printf("ddl values found already, returning"));
                ddlValues_1 = ddlValues;
            }
            toConsole(printf("about to render dropdown from caller"));
            const onChange = (id_2) => {
                toConsole(printf("in onchange handler"));
                setMonster(monsters_2, id_2);
                selectionChanged(id_2);
            };
            const ddl = SearchableWithFunc(monsterDDLProps(onChange, ddlValues_1));
            let deets;
            if (selectedMonster == null) {
                deets = null;
            }
            else {
                const monster = selectedMonster;
                toConsole(printf("viewing monster details for monster %A"))(monster);
                deets = viewMonsterDetails(monster);
            }
            return react.createElement("div", {}, ddl, deets);
        }
    }
    else if (monsters.tag === 3) {
        const e_1 = monsters.fields[0];
        toConsole(printf("in failed handler for deferred"));
        return createElement(ErrorMessage, {
            msg: toText(interpolate("There was an error using deferred data loading: %A%P()", [e_1])),
        });
    }
    else {
        return react.createElement("div", {
            className: "block fa-3x",
        }, Fa_i(ofArray([new Fa_IconOption(11, "fas fa-spinner"), new Fa_IconOption(12)]), []));
    }
});

