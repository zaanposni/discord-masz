import type { IDiscordUser } from "../discord/IDiscordUser";
import type { ICase } from "./ICase";
import type { ICommentView } from "./ICommentView";
import type { IUserNoteView } from "./IUserNoteView";

export interface ICaseView {
    modCase: ICase;
    moderator: IDiscordUser;
    lastModerator: IDiscordUser;
    suspect: IDiscordUser;
    lockedBy?: IDiscordUser;
    deletedBy?: IDiscordUser;
    comments: ICommentView[];
    linkedCases: ICase[];
    userNote: IUserNoteView;
    punishmentProgress: number;
}