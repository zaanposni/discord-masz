import type { WidgetMode } from "../core/dashboard/WidgetMode";

export interface IDashboardItem {
    id: number;
    title: string;
    description: string;
    component: (typeof import("svelte").SvelteComponent);
    mode: WidgetMode;
}