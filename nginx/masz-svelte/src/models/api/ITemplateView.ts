import type { IDiscordGuild } from "../discord/IDiscordGuild";
import type { IDiscordUser } from "../discord/IDiscordUser";
import type { ICaseTemplate } from "./ICaseTemplate";

export interface ITemplateView {
    caseTemplate: ICaseTemplate;
    creator: IDiscordUser;
    guild: IDiscordGuild;
}
