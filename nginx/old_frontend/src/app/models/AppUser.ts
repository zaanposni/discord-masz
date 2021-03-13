import { DiscordUser } from "./DiscordUser";
import { Guild } from "./Guild";

export interface AppUser {
  memberGuilds: Guild[];
  bannedGuilds: Guild[];
  modGuilds: Guild[];
  adminGuilds: Guild[];
  discordUser: DiscordUser;
  isAdmin: boolean;
}