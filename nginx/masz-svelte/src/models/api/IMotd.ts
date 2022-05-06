import type moment from "moment";

export interface IMotd {
    id: number;
    guildId: string;
    userId: string;
    createdAt: moment.Moment;
    message: string;
    showMotd: boolean;
}
