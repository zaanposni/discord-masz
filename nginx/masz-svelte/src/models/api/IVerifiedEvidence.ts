import type moment from "moment";

export interface IVerifiedEvidence {
    id: number;
    guildId: string;
    messageId: string;
    reportedContent: string;
    reporterUserId: string;
    reporterUsername: string;
    reporterNickname?: string;
    reporterDiscriminator: number;
    reportedUserId: string;
    reportedUsername: string;
    reportedNickname?: string;
    reportedDiscriminator: number;
    sentAt: moment.Moment;
    reportedAt: moment.Moment;
}