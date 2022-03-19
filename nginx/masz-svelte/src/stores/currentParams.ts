import { derived, Readable } from "svelte/store";
import { params } from "@roxi/routify";
import { authUser } from "./auth";
import type { IRouteParams } from "../models/IRouteParams";

export const currentParams: Readable<IRouteParams> = derived([params, authUser], ([rParams, currentUser]) => {
    if (!currentUser) {
        return {};
    }

    if (rParams.guildId) {

        // search through all guilds of current user
        return {
            guildId: rParams.guildId,
            guild: currentUser.memberGuilds.find(g => g.id === rParams.guildId) || currentUser.adminGuilds.find(g => g.id === rParams.guildId) || currentUser.modGuilds.find(g => g.id === rParams.guildId) || currentUser.bannedGuilds.find(g => g.id === rParams.guildId) || null
        };
    }
    return {};
}, {});
