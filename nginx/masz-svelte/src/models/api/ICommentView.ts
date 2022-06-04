import type { IDiscordUser } from "../discord/IDiscordUser";
import type { IComment } from "./IComment";

export interface ICommentView {
    comment: IComment;
    commentor?: IDiscordUser;
}
