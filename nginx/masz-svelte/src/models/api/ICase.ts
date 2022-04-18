import type moment from "moment";
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
    createdAt: moment.Moment;
    occuredAt: moment.Moment;
    lastEditedAt: moment.Moment;
    lastEditedByModId?: string;
    labels: string[];
    others?: string;
    valid: boolean;
    creationType: number;
    punishmentType: PunishmentType;
    punishedUntil: moment.Moment;
    punishmentActive: boolean;
    allowComments: boolean;
    lockedAt?: moment.Moment;
    lockedByUserId?: string;
    markedToDeleteAt?: moment.Moment;
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
