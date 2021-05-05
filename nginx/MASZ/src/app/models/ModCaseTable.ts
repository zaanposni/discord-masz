import { DiscordUser } from "./DiscordUser";
import { ModCase } from "./ModCase";

export interface ModCaseTable {
    modCase: ModCase;
    moderator?: DiscordUser;
    suspect?: DiscordUser;
}