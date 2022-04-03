import type { IDiscordUser } from "../discord/IDiscordUser";
import type { IUserMapping } from "./IUserMapping";

export interface IUserMappingView {
    userMapping: IUserMapping;
    userA?: IDiscordUser;
    userB?: IDiscordUser;
    moderator?: IDiscordUser;
}