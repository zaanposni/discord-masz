import { PunishmentType } from "./PunishmentType";

export interface ICase {
    id: number;
    caseId: number;
    guildId: string;
    title: string;
    description: string;
    userId: string;
    username: string;
    discriminator?: string;
    nickname?: string;
    modId?: string;
    createdAt: Date;
    occuredAt: Date;
    lastEditedAt: Date;
    lastEditedByModId?: string;
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
    markedToDeleteAt?: Date;
    deletedByUserId?: string;
    comments: [];
}

export function getI18NPunishment(modCase: ICase) {
    switch(modCase.punishmentType) {
        case PunishmentType.Mute:
            return "enums.punishmenttype.mute";
        case PunishmentType.Kick:
            return "enums.punishmenttype.kick";
        case PunishmentType.Ban:
            return "enums.punishmenttype.ban";
        default:
            return "enums.punishmenttype.warn";
    };
}
