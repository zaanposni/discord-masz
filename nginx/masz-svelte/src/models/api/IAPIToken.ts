import type moment from "moment";

export interface IAPIToken {
    id: number;
    name: string;
    tokenSalt?: any;
    tokenHash?: any;
    createdAt: moment.Moment;
    validUntil: moment.Moment;
}
