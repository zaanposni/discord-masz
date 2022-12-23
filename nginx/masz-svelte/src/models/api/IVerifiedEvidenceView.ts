import type { IDiscordUser } from "../discord/IDiscordUser";
import type { ICase } from "./ICase";
import type { IVerifiedEvidence } from "./IVerifiedEvidence";

export interface IVerifiedEvidenceView {
    evidence: IVerifiedEvidence;
    reporter: IDiscordUser;
    reported: IDiscordUser;
    linkedCases: ICase[];
}