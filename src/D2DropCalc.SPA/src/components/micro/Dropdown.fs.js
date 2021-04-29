import { Record } from "../../.fable/fable-library.3.1.15/Types.js";
import { lambda_type, unit_type, list_type, record_type, string_type } from "../../.fable/fable-library.3.1.15/Reflection.js";
import { React_functionComponent_2F9D7239, useFeliz_React__React_useState_Static_1505 } from "../../.fable/Feliz.1.43.0/React.fs.js";
import { createObj } from "../../.fable/fable-library.3.1.15/Util.js";
import { isNullOrWhiteSpace } from "../../.fable/fable-library.3.1.15/String.js";
import { Interop_defaultProps, SelectItem, selectSearch_options_3A40D30D } from "../../.fable/Feliz.SelectSearch.1.6.0/SelectSearch.fs.js";
import { map, delay, toList } from "../../.fable/fable-library.3.1.15/Seq.js";
import { ofArray } from "../../.fable/fable-library.3.1.15/List.js";
import { Interop_reactApi } from "../../.fable/Feliz.1.43.0/Interop.fs.js";
import react$002Dselect$002Dsearch from "react-select-search";

export class DDLValue extends Record {
    constructor(Value, Name) {
        super();
        this.Value = Value;
        this.Name = Name;
    }
}

export function DDLValue$reflection() {
    return record_type("D2DropCalc.SPA.Components.Micro.Dropdown.DDLValue", [], DDLValue, () => [["Value", string_type], ["Name", string_type]]);
}

export function createDDLValue(value, name) {
    return new DDLValue(value, name);
}

export function Searchable(searchableInputProps) {
    let handler, internalHandler;
    const elements = searchableInputProps.elements;
    const patternInput = useFeliz_React__React_useState_Static_1505(void 0);
    const setSelected = patternInput[1];
    const selectValue = patternInput[0];
    const getValue = (sv) => {
        if (sv == null) {
            return ["placeholder", "Select an option"];
        }
        else {
            const x = sv;
            return ["value", x];
        }
    };
    const inputProperties = createObj(ofArray([getValue(selectValue), ["search", true], (handler = ((value) => {
        setSelected(value);
    }), (internalHandler = ((values) => {
        if (Array.isArray(values)) {
            const unboxed = values;
            if (unboxed.length > 0) {
                handler(unboxed[0]);
            }
        }
        else {
            const unboxed_1 = values;
            if (!isNullOrWhiteSpace(unboxed_1)) {
                handler(unboxed_1);
            }
        }
    }), ["onChange", handler])), selectSearch_options_3A40D30D(toList(delay(() => map((ele) => (new SelectItem(ele.Value, ele.Name, false)), elements))))]));
    return Interop_reactApi.createElement(react$002Dselect$002Dsearch, Object.assign({}, Interop_defaultProps, inputProperties));
}

export function SearchableWithPlaceholder(searchableWithPlaceholderInputProps) {
    let handler, internalHandler;
    const ph = searchableWithPlaceholderInputProps.ph;
    const elements = searchableWithPlaceholderInputProps.elements;
    const patternInput = useFeliz_React__React_useState_Static_1505(void 0);
    const setSelected = patternInput[1];
    const selectValue = patternInput[0];
    const getValue = (sv) => {
        if (sv == null) {
            return ["placeholder", ph];
        }
        else {
            const x = sv;
            return ["value", x];
        }
    };
    const inputProperties = createObj(ofArray([getValue(selectValue), ["search", true], (handler = ((value) => {
        setSelected(value);
    }), (internalHandler = ((values) => {
        if (Array.isArray(values)) {
            const unboxed = values;
            if (unboxed.length > 0) {
                handler(unboxed[0]);
            }
        }
        else {
            const unboxed_1 = values;
            if (!isNullOrWhiteSpace(unboxed_1)) {
                handler(unboxed_1);
            }
        }
    }), ["onChange", handler])), selectSearch_options_3A40D30D(toList(delay(() => map((ele) => (new SelectItem(ele.Value, ele.Name, false)), elements))))]));
    return Interop_reactApi.createElement(react$002Dselect$002Dsearch, Object.assign({}, Interop_defaultProps, inputProperties));
}

export class SearchableProps extends Record {
    constructor(elements, placeholder, callback) {
        super();
        this.elements = elements;
        this.placeholder = placeholder;
        this.callback = callback;
    }
}

export function SearchableProps$reflection() {
    return record_type("D2DropCalc.SPA.Components.Micro.Dropdown.SearchableProps", [], SearchableProps, () => [["elements", list_type(DDLValue$reflection())], ["placeholder", string_type], ["callback", lambda_type(string_type, unit_type)]]);
}

export const SearchableWithFunc = React_functionComponent_2F9D7239((props) => {
    let handler, internalHandler;
    const patternInput = useFeliz_React__React_useState_Static_1505(void 0);
    const setSelected = patternInput[1];
    const selectValue = patternInput[0];
    const getValue = (sv) => {
        if (sv == null) {
            return ["placeholder", props.placeholder];
        }
        else {
            const x = sv;
            return ["value", x];
        }
    };
    const inputProperties = createObj(ofArray([getValue(selectValue), ["search", true], (handler = ((value) => {
        props.callback(value);
        setSelected(value);
    }), (internalHandler = ((values) => {
        if (Array.isArray(values)) {
            const unboxed = values;
            if (unboxed.length > 0) {
                handler(unboxed[0]);
            }
        }
        else {
            const unboxed_1 = values;
            if (!isNullOrWhiteSpace(unboxed_1)) {
                handler(unboxed_1);
            }
        }
    }), ["onChange", handler])), selectSearch_options_3A40D30D(toList(delay(() => map((ele) => (new SelectItem(ele.Value, ele.Name, false)), props.elements))))]));
    return Interop_reactApi.createElement(react$002Dselect$002Dsearch, Object.assign({}, Interop_defaultProps, inputProperties));
});

