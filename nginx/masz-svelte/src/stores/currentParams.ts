import { derived, Readable } from "svelte/store";
import { params } from "@roxi/routify";
import { authUser } from "./auth";
import type { IRouteParams } from "../models/IRouteParams";

let lastGuildId: string | null = null;
export const currentParams: Readable<IRouteParams> = derived([params, authUser], ([rParams, currentUser], set) => {
    if (!currentUser) {
        set({});
        return;
    }

    if (rParams.guildId !== lastGuildId) {
        if (rParams.guildId) {
            lastGuildId = rParams.guildId;
            // search through all guilds of current user
            set({
                guildId: rParams.guildId,
                guild: currentUser.memberGuilds.find(g => g.id === rParams.guildId) || currentUser.adminGuilds.find(g => g.id === rParams.guildId) || currentUser.modGuilds.find(g => g.id === rParams.guildId) || currentUser.bannedGuilds.find(g => g.id === rParams.guildId) || null
            });
        } else {
            lastGuildId = null;
            set({
                guildId: null,
                guild: null
            });
        }
    }
}, {});
