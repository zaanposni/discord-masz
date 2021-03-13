import { CommentView } from "./CommentView";
import { DiscordUser } from "./DiscordUser";
import { ModCase } from "./ModCase";

export interface CaseView {
    modCase: ModCase;
    moderator: DiscordUser;
    lastModerator: DiscordUser;
    suspect: DiscordUser;
    comments: CommentView[];
    punishmentProgress?: number;
}