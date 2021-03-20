import { DiscordUser } from "./DiscordUser";
import { ModCase } from "./ModCase";

export interface CaseComment {
    id: number;
    message: string;
    modCase?: ModCase;
    userId: string;
    createdAt: Date;
}