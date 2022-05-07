export interface IGuildAuditlogConfig {
    id: number;
    guildId: string;
    guildAuditLogEvent: number;
    channelId: string;
    pingRoles: string[];
    ignoreRoles: string[];
    ignoreChannels: string[];
}
