import { DiscordUser } from "./DiscordUser";

export interface GuildMotd {
    id: number;
    guildId: string;
    message: string;
    showMotd: boolean;
    userId: string;
    createdAt: Date;
}

export interface GuildMotdView {
    motd: GuildMotd;
    creator?: DiscordUser;
}
