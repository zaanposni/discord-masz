import { derived, Readable } from "svelte/store";
import { params } from "@roxi/routify";
import { authUser } from "./auth";
import type { IRouteParams } from "../models/IRouteParams";
import type { IDiscordGuild } from "../models/discord/IDiscordGuild";

let lastGuildId: string | null = null;
let lastGuild: IDiscordGuild | null = null;
let lastCaseId: string | null = null;
let lastAppealId: string | null = null;
export const currentParams: Readable<IRouteParams> = derived(
    [params, authUser],
    ([rParams, currentUser], set) => {
        if (!currentUser) {
            set({});
            return;
        }

        const res = {
            guild: lastGuild,
            ...rParams,
        };

        if (rParams.guildId !== lastGuildId) {
            if (rParams.guildId) {
                // search through all guilds of current user
                res.guild =
                    currentUser.memberGuilds.find((g) => g.id === rParams.guildId) ||
                    currentUser.adminGuilds.find((g) => g.id === rParams.guildId) ||
                    currentUser.modGuilds.find((g) => g.id === rParams.guildId) ||
                    currentUser.bannedGuilds.find((g) => g.id === rParams.guildId) ||
                    null;
            } else {
                res.guild = null;
            }
        }

        if (rParams.caseId !== lastCaseId || rParams.guildId !== lastGuildId || rParams.appealId !== lastAppealId) {
            lastGuildId = rParams.guildId;
            lastCaseId = rParams.caseId;
            lastAppealId = rParams.appealId;
            lastGuild = res.guild;
            set(res);
        }
    },
    {}
);
