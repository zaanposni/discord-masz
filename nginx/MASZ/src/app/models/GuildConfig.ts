export interface GuildConfig {
    id: number;
    guildId: string;
    modRoles: string[];
    adminRoles: string[];
    mutedRoles: string[];
    modNotificationDM: boolean;
    strictModPermissionCheck: boolean;
    executeWhoisOnJoin: boolean;
    modPublicNotificationWebhook: string;
    modInternalNotificationWebhook: string;
    publishModeratorInfo: boolean;
    preferredLanguage: number;
}