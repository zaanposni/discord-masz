import { CommentListViewEntry } from "./CommentListViewEntry";
import { DiscordUser } from "./DiscordUser";
import { ModCase } from "./ModCase";
import { UserNoteView } from "./UserNoteView";

export interface CaseView {
    modCase: ModCase;
    moderator?: DiscordUser;
    lastModerator?: DiscordUser;
    suspect?: DiscordUser;
    lockedBy?: DiscordUser;
    deletedBy?: DiscordUser;
    comments: CommentListViewEntry[];
    punishmentProgress?: number;
    userNote?: UserNoteView;
}