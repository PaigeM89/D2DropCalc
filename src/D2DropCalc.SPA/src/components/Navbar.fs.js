import { div } from "../.fable/Fulma.2.10.0/Elements/Form/Control.fs.js";
import { ofArray, singleton, empty } from "../.fable/fable-library.3.1.15/List.js";
import { Option, a } from "../.fable/Fulma.2.10.0/Elements/Button.fs.js";
import { HTMLAttr } from "../.fable/Fable.React.7.4.0/Fable.React.Props.fs.js";
import { icon as icon_1 } from "../.fable/Fulma.2.10.0/Elements/Icon.fs.js";
import { Fa_IconOption, Fa_i } from "../.fable/Fable.FontAwesome.2.0.0/FontAwesome.fs.js";
import * as react from "react";
import { Item_div, Start_div, Option as Option_1, navbar } from "../.fable/Fulma.2.10.0/Components/Navbar.fs.js";
import { Color_IColor } from "../.fable/Fulma.2.10.0/Common.fs.js";
import { container } from "../.fable/Fulma.2.10.0/Layouts/Container.fs.js";
import { Option as Option_2, h3 } from "../.fable/Fulma.2.10.0/Elements/Heading.fs.js";

function SubComponents_navButton(href, icon, txt) {
    return div(empty(), singleton(a(singleton(new Option(16, singleton(new HTMLAttr(94, href)))), ofArray([icon_1(empty(), singleton(Fa_i(singleton(icon), []))), react.createElement("span", {}, txt)]))));
}

export function Navbar() {
    return navbar(singleton(new Option_1(0, new Color_IColor(19, "diablo-red"))), singleton(container(empty(), ofArray([Start_div(empty(), singleton(h3(singleton(new Option_2(8, "site-title")))(singleton("Diablo 2 Drop Calculator")))), Item_div(empty(), singleton(SubComponents_navButton("https://github.com/PaigeM89/D2DropCalc", new Fa_IconOption(11, "fab fa-github"), "Github")))]))));
}

