import type { IDiscordUser } from "../discord/IDiscordUser";
import type { ICase } from "./ICase";

export interface ICompactCaseView {
    modCase: ICase;
    moderator: IDiscordUser;
    suspect: IDiscordUser;
}