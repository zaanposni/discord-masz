import { DiscordUser } from "./DiscordUser";

export interface GuildMotd {
    id: number;
    guildId: string;
    message: string;
    userId: string;
    createdAt: Date;
}

export interface GuildMotdView {
    motd: GuildMotd;
    creator: DiscordUser;
}
