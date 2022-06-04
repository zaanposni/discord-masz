import type { Readable, Writable } from "svelte/store";
import { writable, derived } from "svelte/store";

export const currentTheme: Writable<string> = writable(null);
export const isDarkMode: Readable<boolean> = derived(currentTheme, (theme) => {
    return theme !== "white" && theme !== "g10";
});
