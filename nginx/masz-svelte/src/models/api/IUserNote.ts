import type moment from "moment";

export interface IUserNote {
    id: number;
    guildId: string;
    userId: string;
    description: string;
    creatorId: string;
    updatedAt: moment.Moment;
}
