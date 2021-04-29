import { createElement } from "react";
import * as react from "react";
import { Navbar } from "./components/Navbar.fs.js";
import { columns } from "./.fable/Fulma.2.10.0/Layouts/Columns.fs.js";
import { singleton, empty } from "./.fable/fable-library.3.1.15/List.js";
import { column } from "./.fable/Fulma.2.10.0/Layouts/Column.fs.js";
import { container } from "./.fable/Fulma.2.10.0/Layouts/Container.fs.js";
import { SelectAndViewItem } from "./components/ViewItem.fs.js";
import { render } from "react-dom";

export function RootComponent() {
    return react.createElement("div", {}, createElement(Navbar, null), columns(empty(), singleton(column(empty(), singleton(container(empty(), singleton(SelectAndViewItem())))))));
}

render(createElement(RootComponent, null), document.getElementById("root"));

