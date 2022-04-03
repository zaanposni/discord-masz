import type { IDiscordUser } from "../discord/IDiscordUser";
import type { IUserNote } from "./IUserNote";

export interface IUserNoteView {
    userNote: IUserNote;
    user?: IDiscordUser;
    moderator?: IDiscordUser;
}