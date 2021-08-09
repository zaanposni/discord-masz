import { Guild } from "./Guild";
import { UserInviteView } from "./UserInviteView";

export interface InviteNetwork {
    guild?: Guild;
    invites: UserInviteView[];
}