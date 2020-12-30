import { DiscordUser } from "./DiscordUser";

export interface CaseComment {
    id: number;
    message: string;
    userId: string;
    createdAt: Date;
}