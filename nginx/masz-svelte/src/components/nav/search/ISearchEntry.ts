import type { GotoHelper } from "@roxi/routify";
import type { IAuthUser } from "../../../models/IAuthUser";
import type { IRouteParams } from "../../../models/IRouteParams";

export interface ISearchEntryDefinition {
    textKey: string;
    descriptionKey?: string;
    onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => void;
    additionalSearchKeys?: string[];
    allowedToView?: (authUser: IAuthUser, params: IRouteParams) => boolean;
    href?: string;
    hidePerDefault?: boolean;
}

export interface ISearchEntry {
    text: string;
    description?: string;
    onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => void;
    additionalSearch?: string;
    allowedToView?: (authUser: IAuthUser, params: IRouteParams) => boolean;
    href: string;
    hidePerDefault?: boolean;
}

