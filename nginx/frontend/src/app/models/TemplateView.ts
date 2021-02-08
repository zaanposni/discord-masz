import { CaseTemplate } from "./CaseTemplate";
import { DiscordUser } from "./DiscordUser";
import { Guild } from "./Guild";

export interface TemplateView {
    caseTemplate: CaseTemplate;
    creator: DiscordUser;
    guild: Guild;
}