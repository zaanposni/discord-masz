import type { Writable } from "svelte/store";
import { writable } from "svelte/store";
import { LOCAL_STORAGE_KEY_FAVORITE_GUILD } from "../config";

export const firstVisitOnGuildList: Writable<boolean> = writable(true);
export const favoriteGuild: Writable<string> = writable(localStorage.getItem(LOCAL_STORAGE_KEY_FAVORITE_GUILD) || "");

favoriteGuild.subscribe(guildId => {
    if (guildId) {
        localStorage.setItem(LOCAL_STORAGE_KEY_FAVORITE_GUILD, guildId);
    } else {
        localStorage.removeItem(LOCAL_STORAGE_KEY_FAVORITE_GUILD);
    }
});
