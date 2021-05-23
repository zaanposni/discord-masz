export interface Adminstats {
    loginsInLast15Minutes: string[];
    trackedInvites: number;
    modCases: number;
    guilds: number;
    automodEvents: number;
    userNotes: number;
    userMappings: number;
    apiTokens: number;
    nextCache?: Date;
    cachedDataFromDiscord: string[];
}