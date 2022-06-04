import type { WidgetMode } from "../core/dashboard/WidgetMode";

export interface IDashboardItem {
    id: string;
    translationKey: string;
    component: (typeof import("svelte").SvelteComponent);
    mode: WidgetMode;
    fix?: boolean
}