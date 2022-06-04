import type { IDiscordUser } from "../discord/IDiscordUser";
import type { IMotd } from "./IMotd";

export interface IMotdView {
    motd: IMotd;
    creator: IDiscordUser;
}
