import { CommentListViewEntry } from "./CommentListViewEntry";
import { DiscordUser } from "./DiscordUser";
import { ModCase } from "./ModCase";

export interface CaseView {
    modCase: ModCase;
    moderator?: DiscordUser;
    lastModerator?: DiscordUser;
    suspect?: DiscordUser;
    lockedBy?: DiscordUser;
    deletedBy?: DiscordUser;
    comments: CommentListViewEntry[];
    punishmentProgress?: number;
}