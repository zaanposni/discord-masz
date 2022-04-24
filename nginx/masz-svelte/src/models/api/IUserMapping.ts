import type moment from "moment";

export interface IUserMapping {
    id: number;
    guildId: string;
    userA: string;
    userB: string;
    creatorUserId: string;
    createdAt: moment.Moment;
    reason: string;
}