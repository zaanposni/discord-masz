import type moment from "moment";

export interface IMASZVersion {
    id: number;
    imagename: string;
    createdAt: moment.Moment;
    tag: string;
    hash: string;
}
