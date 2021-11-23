import { AutoModerationAction } from "./AutoModerationAction";
import { AutoModerationType } from "./AutoModerationType";
import { ChannelNotificationBehavior } from "./ChannelNotificationBehavior";
import { PunishmentType } from "./PunishmentType";

export interface AutoModerationConfig {
    id: number;
    guildId: string;
    autoModerationType: AutoModerationType;
    autoModerationAction: AutoModerationAction;
    punishmentType: PunishmentType;
    punishmentDurationMinutes: number;
    ignoreChannels: string[];
    ignoreRoles: string[];
    timeLimitMinutes?: number;
    limit?: number;
    customWordFilter?: string;
    sendDmNotification: boolean;
    sendPublicNotification: boolean;
    channelNotificationBehavior: ChannelNotificationBehavior;
}

