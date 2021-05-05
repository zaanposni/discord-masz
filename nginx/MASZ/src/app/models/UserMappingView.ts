import { DiscordUser } from "./DiscordUser";
import { UserMapping } from "./UserMapping";

export interface UserMappingView {
    userMapping: UserMapping;
    userA?: DiscordUser;
    userB?: DiscordUser;
    moderator?: DiscordUser;
}