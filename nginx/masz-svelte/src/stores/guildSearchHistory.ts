import { derived, Readable, writable, Writable } from "svelte/store";
import { GUILD_QUICKSEARCH_MAX_HISTORY_ENTRIES, LOCAL_STORAGE_KEY_GUILD_QUICKSEARCH_HISTORY } from "../config";
import type { IGuildSearchHistory } from "../models/IGuildSearchHistory";
import { currentParams } from "./currentParams";

const fullSearchHistory: Writable<IGuildSearchHistory[]> = writable(JSON.parse(localStorage.getItem(LOCAL_STORAGE_KEY_GUILD_QUICKSEARCH_HISTORY) || "[]"));
export const searchHistory: Readable<string[]> = derived([fullSearchHistory, currentParams], ([searches, params]) => {
    if (params.guildId) {
        return searches.find(search => search.guildId === params.guildId)?.searches ?? [];
    } else {
        return [];
    }
}, []);

export function clearSearchHistory(guildId: string) {
    fullSearchHistory.update(searches => {
        searches = searches.filter(x => x.guildId !== guildId);
        return searches;
    });
}

export function putInSearchHistory(guildId: string, search: string) {
    fullSearchHistory.update(searches => {
        const index = searches.findIndex(x => x.guildId === guildId);
        if (index !== -1) {
            if (!searches[index].searches.includes(search)) {
                searches[index].searches.unshift(search);
                searches[index].searches = searches[index].searches.slice(0, GUILD_QUICKSEARCH_MAX_HISTORY_ENTRIES);
            }
        } else {
            searches.push({
                guildId,
                searches: [search],
            });
        }
        return searches;
    });
}

fullSearchHistory.subscribe(searches => {
    localStorage.setItem(LOCAL_STORAGE_KEY_GUILD_QUICKSEARCH_HISTORY, JSON.stringify(searches));
});



