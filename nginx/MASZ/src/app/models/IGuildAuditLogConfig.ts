export interface IGuildAuditLogConfig {
    id: number;
    guildId: string;
    guildAuditLogEvent: number;
    channelId: string;
    pingRoles: string[];
}