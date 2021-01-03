export interface AutomodConfig {
    id: number;
    guildId: string;
    autoModerationType: number;
    autoModerationAction: number;
    punishmentType: number;
    punishmentDurationMinutes: number;
    ignoreChannels: string[];
    ignoreRoles: string[];
    timeLimitMinutes?: any;
    limit?: any;
    sendDmNotification: boolean;
    sendPublicNotification: boolean;
}