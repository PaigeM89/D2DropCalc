import { int32ToString } from "../.fable/fable-library.3.1.15/Util.js";
import { createElement } from "react";
import * as react from "react";
import { map, ofArray, singleton } from "../.fable/fable-library.3.1.15/List.js";
import { Interop_reactApi } from "../.fable/Feliz.1.43.0/Interop.fs.js";
import { toString } from "../.fable/fable-library.3.1.15/Types.js";
import { useReact_useEffect_Z101E1A95, useFeliz_React__React_useState_Static_1505 } from "../.fable/Feliz.1.43.0/React.fs.js";
import { useFeliz_React__React_useDeferredCallback_Static_7088D81D, Deferred$1 } from "../.fable/Feliz.UseDeferred.1.4.1/UseDeferred.fs.js";
import { interpolate, toText, printf, toConsole } from "../.fable/fable-library.3.1.15/String.js";
import { getArmors } from "../Fetch.fs.js";
import { Fa_IconOption, Fa_i } from "../.fable/Fable.FontAwesome.2.0.0/FontAwesome.fs.js";
import { table, TableOption } from "../.fable/Fulma.2.10.0/Elements/Table.fs.js";
import { delay, toList } from "../.fable/fable-library.3.1.15/Seq.js";

export function armorTableRow(armor) {
    let children, children_2, children_4, value_2, children_6, value_3, children_8, value_4, children_10, value_5, children_12, value_6;
    const tostr = (io) => {
        if (io == null) {
            return "-";
        }
        else {
            const i = io | 0;
            return int32ToString(i);
        }
    };
    const children_14 = ofArray([(children = singleton(createElement("p", {
        children: [armor.Name],
    })), createElement("td", {
        children: Interop_reactApi.Children.toArray(Array.from(children)),
    })), (children_2 = singleton(createElement("p", {
        children: [armor.Code],
    })), createElement("td", {
        children: Interop_reactApi.Children.toArray(Array.from(children_2)),
    })), (children_4 = singleton((value_2 = tostr(armor.ItemLevel), createElement("p", {
        children: [value_2],
    }))), createElement("td", {
        children: Interop_reactApi.Children.toArray(Array.from(children_4)),
    })), (children_6 = singleton((value_3 = tostr(armor.ReqLevel), createElement("p", {
        children: [value_3],
    }))), createElement("td", {
        children: Interop_reactApi.Children.toArray(Array.from(children_6)),
    })), (children_8 = singleton((value_4 = tostr(armor.ReqStrength), createElement("p", {
        children: [value_4],
    }))), createElement("td", {
        children: Interop_reactApi.Children.toArray(Array.from(children_8)),
    })), (children_10 = singleton((value_5 = tostr(armor.MaxSockets), createElement("p", {
        children: [value_5],
    }))), createElement("td", {
        children: Interop_reactApi.Children.toArray(Array.from(children_10)),
    })), (children_12 = singleton((value_6 = toString(armor.BaseType), createElement("p", {
        children: [value_6],
    }))), createElement("td", {
        children: Interop_reactApi.Children.toArray(Array.from(children_12)),
    }))]);
    return createElement("tr", {
        children: Interop_reactApi.Children.toArray(Array.from(children_14)),
    });
}

export function armorTableHeader() {
    let children, children_2, children_4, children_6, children_8, children_10, children_12;
    const children_14 = ofArray([(children = singleton(createElement("h3", {
        children: ["Name"],
    })), createElement("th", {
        children: Interop_reactApi.Children.toArray(Array.from(children)),
    })), (children_2 = singleton(createElement("h3", {
        children: ["Code"],
    })), createElement("th", {
        children: Interop_reactApi.Children.toArray(Array.from(children_2)),
    })), (children_4 = singleton(createElement("h3", {
        children: ["Item Level"],
    })), createElement("th", {
        children: Interop_reactApi.Children.toArray(Array.from(children_4)),
    })), (children_6 = singleton(createElement("h3", {
        children: ["Req Level"],
    })), createElement("th", {
        children: Interop_reactApi.Children.toArray(Array.from(children_6)),
    })), (children_8 = singleton(createElement("h3", {
        children: ["Req Strength"],
    })), createElement("th", {
        children: Interop_reactApi.Children.toArray(Array.from(children_8)),
    })), (children_10 = singleton(createElement("h3", {
        children: ["Max Sockets"],
    })), createElement("th", {
        children: Interop_reactApi.Children.toArray(Array.from(children_10)),
    })), (children_12 = singleton(createElement("h3", {
        children: ["Base Item Type"],
    })), createElement("th", {
        children: Interop_reactApi.Children.toArray(Array.from(children_12)),
    }))]);
    return createElement("tr", {
        children: Interop_reactApi.Children.toArray(Array.from(children_14)),
    });
}

export function LoadArmors() {
    const patternInput = useFeliz_React__React_useState_Static_1505(new Deferred$1(0));
    const setArmors = patternInput[1];
    const armors = patternInput[0];
    toConsole(printf("in load armors component"));
    const loadData = getArmors();
    const startLoading = useFeliz_React__React_useDeferredCallback_Static_7088D81D(() => loadData, setArmors);
    useReact_useEffect_Z101E1A95(startLoading, []);
    if (armors.tag === 1) {
        return react.createElement("div", {
            className: "block fa-3x",
        }, Fa_i(ofArray([new Fa_IconOption(11, "fas fa-spinner"), new Fa_IconOption(12)]), []));
    }
    else if (armors.tag === 3) {
        const error = armors.fields[0];
        const value_1 = error.message;
        return createElement("h1", {
            children: [value_1],
        });
    }
    else if (armors.tag === 2) {
        if (armors.fields[0].tag === 1) {
            const fetchError = armors.fields[0].fields[0];
            const value_2 = toText(interpolate("Error fetching data: %A%P()", [fetchError]));
            return createElement("h1", {
                children: [value_2],
            });
        }
        else {
            const content = armors.fields[0].fields[0];
            const tableProps = ofArray([new TableOption(0), new TableOption(1)]);
            const head = armorTableHeader();
            const rows = map((armor) => armorTableRow(armor), content);
            return table(toList(delay(() => tableProps)), ofArray([react.createElement("thead", {}, head), react.createElement("tbody", {}, ...rows)]));
        }
    }
    else {
        return createElement("h1", {
            children: ["Not Started"],
        });
    }
}

