import { derived, writable } from "svelte/store";
import type { Readable, Writable } from "svelte/store";
import type { IDashboardItem } from "../models/IDashboardItem";
import { LOCAL_STORAGE_KEY_ADMIN_DASHBOARD_ITEMS } from "../config";

export const adminDashboardClearingCache: Writable<boolean> = writable(false);
export const adminDashboardEnableDragging: Writable<boolean> = writable(false);
export const adminDashboardItems: Writable<IDashboardItem[]> = writable([]);
export const adminDashboardToggledItems: Writable<{ id: string; enabled: boolean, sortOrder: number }[]> = writable(
    JSON.parse(localStorage.getItem(LOCAL_STORAGE_KEY_ADMIN_DASHBOARD_ITEMS) || "[]")
);
export const visibleAdminDashboardItems: Readable<IDashboardItem[]> = derived(
    [adminDashboardItems, adminDashboardToggledItems],
    ([items, toggledItems]) => {
        if (toggledItems.length === 0) {
            return items;
        }

        const result = [];

        // sort toggledItems based on sortOrder
        toggledItems = toggledItems.sort((a, b) => a.sortOrder - b.sortOrder);

        for (let item of toggledItems) {
            const entry = items.find((x) => x.id === item.id && item.enabled);
            if (entry) {
                result.push(entry);
            }
        }

        for (let item of items.filter(x => x.fix)) {
            if (!result.find(x => x.id === item.id)) {
                result.push(item);
            }
        }

        return result;
    },
    []
);

adminDashboardToggledItems.subscribe((items) => {
    if (items?.length) {
        localStorage.setItem(LOCAL_STORAGE_KEY_ADMIN_DASHBOARD_ITEMS, JSON.stringify(items));
    } else {
        localStorage.removeItem(LOCAL_STORAGE_KEY_ADMIN_DASHBOARD_ITEMS);
    }
});
