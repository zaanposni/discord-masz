import type { IDiscordUser } from "../discord/IDiscordUser";
import type { ICase } from "./ICase";

export interface IVerifiedEvidenceView {
    evidence: IVerifiedEvidenceView;
    reporter: IDiscordUser;
    reported: IDiscordUser;
    linkedCases: ICase[];
}