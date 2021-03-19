import { CaseComment } from "./CaseComment";

export interface ModCase {
    id: number;
    caseId: number;
    guildId: string;
    title: string;
    description: string;
    userId: string;
    username: string;
    discriminator?: string;
    nickname?: string;
    modId: string;
    createdAt: Date;
    occuredAt: Date;
    lastEditedAt: Date;
    lastEditedByModId: string;
    punishment: string;
    labels: string[];
    others?: string;
    valid: boolean;
    creationType: number;
    punishmentType: PunishmentType;
    punishedUntil: Date;
    punishmentActive: boolean;
    allowComments: boolean;
    lockedAt: Date;
    lockedByUserId: string;
    comments: CaseComment[];
}

export enum PunishmentType {
    None,
    Mute,
    Kick,
    Ban
}

export enum DisplayPunishmentType {
    None,
    Warn,
    Notice,
    Mute,
    TempMute,
    Kick,
    Ban,
    TempBan
}

export function DisplayPunishmentTypeOptions() : Array<string> {
    var keys = Object.keys(DisplayPunishmentType);
    return keys.slice(keys.length / 2);
}

export function convertToDisplayPunishmentType(type: PunishmentType, punishment: string, until: Date|undefined): DisplayPunishmentType {
    switch(type) {
        case PunishmentType.None:
            switch(punishment.trim().toLowerCase()) {
                case 'notice':
                    return DisplayPunishmentType.Notice;
                case 'none':
                    return DisplayPunishmentType.None;
                default:
                    return DisplayPunishmentType.Warn;
            }
        case PunishmentType.Kick:
            return DisplayPunishmentType.Kick;
        case PunishmentType.Mute:
            if (until) {
                return DisplayPunishmentType.TempMute;
            } else {
                return DisplayPunishmentType.Mute;
            }
        case PunishmentType.Ban:
            if (until) {
                return DisplayPunishmentType.TempBan;
            } else {
                return DisplayPunishmentType.Ban;
            }
        default:
            return DisplayPunishmentType.Warn;
    }
}

export function convertToPunishmentType(type: DisplayPunishmentType): PunishmentType {
    switch(type) {
        case DisplayPunishmentType.Kick:
            return PunishmentType.Kick;
        case DisplayPunishmentType.Mute:
            return PunishmentType.Mute;
        case DisplayPunishmentType.TempMute:
            return PunishmentType.Mute;
        case DisplayPunishmentType.Ban:
            return PunishmentType.Ban;
        case DisplayPunishmentType.TempBan:
            return PunishmentType.Ban;
        default:
            return PunishmentType.None;
    }
}

export function convertToPunishment(type: DisplayPunishmentType): string {
    switch(type) {
        case DisplayPunishmentType.Kick:
            return 'Kick';
        case DisplayPunishmentType.Mute:
            return 'Mute';
        case DisplayPunishmentType.TempMute:
            return 'TempMute';
        case DisplayPunishmentType.Ban:
            return 'Ban';
        case DisplayPunishmentType.TempBan:
            return 'TempBan';
        case DisplayPunishmentType.Notice:
            return 'Notice';
        case DisplayPunishmentType.Warn:
            return 'Warn';
        default:
            return 'None';
    }
}
