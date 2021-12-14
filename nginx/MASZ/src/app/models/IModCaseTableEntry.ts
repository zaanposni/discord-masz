import { DiscordUser } from "./DiscordUser";
import { ModCase } from "./ModCase";

export interface IModCaseTableEntry {
    modCase: ModCase;
    moderator?: DiscordUser;
    suspect?: DiscordUser;
}