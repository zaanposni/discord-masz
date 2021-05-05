import { DiscordUser } from "./DiscordUser";
import { UserMapping } from "./Usermapping";

export interface RootObject {
    userMapping: UserMapping;
    userA?: DiscordUser;
    userB?: DiscordUser;
    moderator?: DiscordUser;
}