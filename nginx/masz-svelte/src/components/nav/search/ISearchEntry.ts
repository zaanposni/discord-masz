import type { GotoHelper } from "@roxi/routify";
import type { IAuthUser } from "../../../models/IAuthUser";

export interface ISearchEntryDefinition {
    textKey: string;
    descriptionKey?: string;
    onSelect: (gotoHelper: GotoHelper) => void;
    additionalSearchKeys?: string[];
    allowedToView?: (authUser: IAuthUser) => boolean;
    href?: string;
}

export interface ISearchEntry {
    text: string;
    description?: string;
    onSelect: (gotoHelper: GotoHelper) => void;
    additionalSearch?: string;
    allowedToView?: (authUser: IAuthUser) => boolean;
    href: string;
}

