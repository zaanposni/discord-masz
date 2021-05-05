import { DiscordUser } from "./DiscordUser";
import { UserNote } from "./UserNote";

export interface UserNoteView {
    userNote: UserNote;
    user?: DiscordUser;
    moderator?: DiscordUser;
}