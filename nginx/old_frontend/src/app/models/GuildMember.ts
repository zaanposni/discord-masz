import { DiscordUser } from "./DiscordUser";

export interface GuildMember {
    user: DiscordUser;
    nick?: string;
    roles?: string[];
    joined_at?: Date;
}