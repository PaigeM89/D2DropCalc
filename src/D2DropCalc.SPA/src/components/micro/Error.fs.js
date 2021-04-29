import { body, header, Option, message } from "../../.fable/Fulma.2.10.0/Components/Message.fs.js";
import { Color_IColor } from "../../.fable/Fulma.2.10.0/Common.fs.js";
import { ofArray, empty, singleton } from "../../.fable/fable-library.3.1.15/List.js";

export function ErrorMessage(errorMessageInputProps) {
    const msg = errorMessageInputProps.msg;
    return message(singleton(new Option(0, new Color_IColor(8))), ofArray([header(empty(), singleton("Error")), body(empty(), singleton(msg))]));
}

