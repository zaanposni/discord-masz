import type { AutomodAction } from "./AutomodActionEnum";
import type { AutomodType } from "./AutomodType";
import type { PunishmentType } from "./PunishmentType";

export interface IAutomodConfig {
    id: number;
    guildId: string;
    autoModerationType: AutomodType;
    autoModerationAction: AutomodAction;
    punishmentType?: PunishmentType;
    punishmentDurationMinutes?: number;
    ignoreChannels: string[];
    ignoreRoles: string[];
    timeLimitMinutes?: number;
    limit?: number;
    customWordFilter?: string;
    sendDmNotification: boolean;
    sendPublicNotification: boolean;
    channelNotificationBehavior: number;
}
