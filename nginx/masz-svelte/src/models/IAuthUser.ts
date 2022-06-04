import type { IDiscordGuild } from "./discord/IDiscordGuild";
import type { IDiscordUser } from "./discord/IDiscordUser";

export interface IAuthUser {
    memberGuilds: IDiscordGuild[];
    bannedGuilds: IDiscordGuild[];
    modGuilds: IDiscordGuild[];
    adminGuilds: IDiscordGuild[];
    discordUser: IDiscordUser;
    isAdmin: boolean;
}