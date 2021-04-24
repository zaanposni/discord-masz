import { DiscordUser } from "./DiscordUser";
import { UserInvite } from "./UserInvite";

export interface UserInviteView {
    userInvite: UserInvite;
    invitedUser?: DiscordUser;
    invitedBy?: DiscordUser;
}