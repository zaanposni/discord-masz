import type { IDiscordUser } from "../discord/IDiscordUser";
import type { ICase } from "./ICase";
import type { IVerifiedEvidence } from "./IVerifiedEvidence";

export interface IVerifiedEvidenceView {
    evidence: IVerifiedEvidence;
    reported: IDiscordUser;
    moderator: IDiscordUser;
    linkedCases: ICase[];
}