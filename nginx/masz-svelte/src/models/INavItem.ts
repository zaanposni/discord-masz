import type { IAuthUser } from "./IAuthUser";
import type { IRouteParams } from "./IRouteParams";

export interface INavItem {
    titleKey: string;
    onClick?: () => void;
    href?: string;
    checkSelected?: string;
    isDivider?: boolean;
    isAllowedToView?: (authUser: IAuthUser, params: IRouteParams) => boolean;
    icon?: (typeof import("svelte").SvelteComponent);
}