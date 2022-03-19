import type { IDiscordGuild } from "./discord/IDiscordGuild";

export interface IRouteParams {
    guildId?: string;
    guild?: IDiscordGuild
}
