export interface IGuildConfig {
    id: number;
    guildId: string;
    modRoles: string[];
    adminRoles: string[];
    mutedRoles: string[];
    modNotificationDM: boolean;
    modPublicNotificationWebhook: string;
    modInternalNotificationWebhook: string;
    strictModPermissionCheck: boolean;
    executeWhoisOnJoin: boolean;
    publishModeratorInfo: boolean;
    preferredLanguage: number;
    allowBanAppealAfterDays: number;
    publicEmbedMode: boolean;
}