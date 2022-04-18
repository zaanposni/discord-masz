import type moment from "moment";

export interface IComment {
    id: number;
    message: string;
    createdAt: moment.Moment;
    userId: string;
}
