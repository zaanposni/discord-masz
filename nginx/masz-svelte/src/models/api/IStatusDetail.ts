import type moment from "moment";

export interface IStatusDetail {
    online: boolean;
    lastDisconnect?: moment.Moment;
    responseTime: number;
    message?: string;
}
