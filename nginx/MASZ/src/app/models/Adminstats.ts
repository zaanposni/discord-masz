import { IServiceStatus } from "./IServiceStatus";

export interface Adminstats {
    botStatus: IServiceStatus;
    dbStatus: IServiceStatus;
    cacheStatus: IServiceStatus;
    loginsInLast15Minutes: string[];
    defaultLanguage: number;
    trackedInvites: number;
    modCases: number;
    guilds: number;
    automodEvents: number;
    userNotes: number;
    userMappings: number;
    apiTokens: number;
    nextCache: Date;
    cachedDataFromDiscord: string[];
}