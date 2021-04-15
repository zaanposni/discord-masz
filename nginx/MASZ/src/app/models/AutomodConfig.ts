import { AutoModerationType } from "./AutoModerationType";

export interface AutomodConfig {
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
    sendDmNotification: boolean;
    sendPublicNotification: boolean;
}

export enum AutoModerationAction {
    None,
    ContentDeleted,
    CaseCreated,
    ContentDeletedAndCaseCreated
}

export enum PunishmentType {
    None,
    Mute,
    Kick,
    Ban
}

export enum AutoModerationPunishment {
    Warn,
    Kick,
    Mute,
    Ban,
    TempMute,
    TempBan
}

export function AutoModerationActionOptions() : Array<string> {
    var keys = Object.keys(AutoModerationAction);
    return keys.slice(keys.length / 2);
}

export function AutoModerationPunishmentOptions() : Array<string> {
    var keys = Object.keys(AutoModerationPunishment);
    return keys.slice(keys.length / 2);
}

export function convertToAutoModPunishment(punishmentType: PunishmentType, punishmentDurationMinutes: number|null): AutoModerationPunishment {
    switch(punishmentType) {
        case PunishmentType.None:
            return AutoModerationPunishment.Warn;
        case PunishmentType.Kick:
            return AutoModerationPunishment.Kick;
        case PunishmentType.Mute:
            if (punishmentDurationMinutes) {
                return AutoModerationPunishment.TempMute;
            } else {
                return AutoModerationPunishment.Mute;
            }
        case PunishmentType.Ban:
            if (punishmentDurationMinutes) {
                return AutoModerationPunishment.TempBan;
            } else {
                return AutoModerationPunishment.Ban;
            }
        default:
            return AutoModerationPunishment.Warn;
    }
}

export function convertToPunishmentType(type: AutoModerationPunishment) {
    switch(type) {
        case AutoModerationPunishment.Kick:
            return PunishmentType.Kick;
        case AutoModerationPunishment.Mute:
            return PunishmentType.Mute;
        case AutoModerationPunishment.TempMute:
            return PunishmentType.Mute;
        case AutoModerationPunishment.Ban:
            return PunishmentType.Ban;
        case AutoModerationPunishment.TempBan:
            return PunishmentType.Ban;
        default:
            return PunishmentType.None;
    }
}
