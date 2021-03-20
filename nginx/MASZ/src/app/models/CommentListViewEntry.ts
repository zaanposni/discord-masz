import { CaseComment } from "./CaseComment";
import { DiscordUser } from "./DiscordUser";

export interface CommentListViewEntry {
    comment: CaseComment;
    commentor: DiscordUser;
}