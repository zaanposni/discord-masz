import { DiscordUser } from "./DiscordUser";

export interface CommentView {
    id: number;
    message: string;
    createdAt: Date;
    userId: string;
    user: DiscordUser;
}