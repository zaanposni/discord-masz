import type { IDiscordRole } from "./IDiscordRole";

export interface IDiscordGuild {
    id: string;
    name: string;
    iconUrl: string;
    roles: IDiscordRole[];
}