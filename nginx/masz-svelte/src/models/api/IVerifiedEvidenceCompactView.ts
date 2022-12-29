import type { IDiscordUser } from "../discord/IDiscordUser";
import type { IVerifiedEvidence } from "./IVerifiedEvidence";

export interface IVerifiedEvidenceCompactView {
    verifiedEvidence: IVerifiedEvidence;
    reported: IDiscordUser;
    moderator: IDiscordUser;
}