import { derived, writable } from "svelte/store";
import type { Readable, Writable } from "svelte/store";
import type { IDashboardItem } from "../models/IDashboardItem";
import { LOCAL_STORAGE_KEY_GUILD_DASHBOARD_ITEMS } from "../config";

export const guildDashboardItems: Writable<IDashboardItem[]> = writable([]);
export const guildDashboardToggledItems: Writable<{ id: string; enabled: boolean }[]> = writable(
    JSON.parse(localStorage.getItem(LOCAL_STORAGE_KEY_GUILD_DASHBOARD_ITEMS) || "[]")
);
export const visibleGuildDashboardItems: Readable<IDashboardItem[]> = derived(
    [guildDashboardItems, guildDashboardToggledItems],
    ([items, toggledItems]) => {
        const result = [];
        for (let item of items) {
            const entry = toggledItems.find((x) => x.id === item.id);
            if (entry === undefined || entry?.enabled || item?.fix) {
                result.push(item);
            }
        }
        return result;
    },
    []
);

guildDashboardToggledItems.subscribe((items) => {
    if (items?.length) {
        localStorage.setItem(LOCAL_STORAGE_KEY_GUILD_DASHBOARD_ITEMS, JSON.stringify(items));
    } else {
        localStorage.removeItem(LOCAL_STORAGE_KEY_GUILD_DASHBOARD_ITEMS);
    }
});
