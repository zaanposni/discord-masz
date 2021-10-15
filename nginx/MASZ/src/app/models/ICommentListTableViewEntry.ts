import { CommentListViewEntry } from "./CommentListViewEntry";

export interface ICommentListTableViewEntry extends CommentListViewEntry {
    guildId: string;
    caseId: number;
}