import type moment from "moment";
import type { IStatusDetail } from "./IStatusDetail";

export interface IAdminStats {
    botStatus: IStatusDetail;
    dbStatus: IStatusDetail;
    cacheStatus: IStatusDetail;
    loginsInLast15Minutes: string[];
    defaultLanguage: number;
    trackedInvites: number;
    modCases: number;
    guilds: number;
    automodEvents: number;
    userNotes: number;
    userMappings: number;
    apiTokens: number;
    scheduledMessages: number;
    appeals: number;
    nextCache: moment.Moment;
    cachedDataFromDiscord: string[];
}
