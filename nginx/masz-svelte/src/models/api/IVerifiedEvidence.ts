import type moment from "moment";

export interface IVerifiedEvidence {
    id: number;
    guildId: string;
    channelId: string;
    messageId: string;
    reportedContent: string;
    userId: string;
    username: string;
    nickname?: string;
    modId: string;
    sentAt: moment.Moment;
    reportedAt: moment.Moment;
}