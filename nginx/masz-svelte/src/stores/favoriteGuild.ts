import type { Writable } from "svelte/store";
import { writable } from "svelte/store";

export const firstVisitOnGuildList: Writable<boolean> = writable(true);
export const favoriteGuild: Writable<string> = writable(localStorage.getItem("favoriteGuild") || "");

favoriteGuild.subscribe(guildId => {
    localStorage.setItem("favoriteGuild", guildId);
});
