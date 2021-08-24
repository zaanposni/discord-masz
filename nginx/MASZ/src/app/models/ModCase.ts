import { CaseComment } from "./CaseComment";
import { PunishmentType } from "./PunishmentType";

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
    comments: CaseComment[];
}

export function convertModcaseToPunishmentString(modcase?: ModCase): string {
    if (!modcase) {
        return "Unknown";
    }
    switch (modcase.punishmentType) {
        case PunishmentType.None:
            return "Warn";
        case PunishmentType.Mute:
            if (modcase.punishedUntil) {
                return "TempMute"
            }
            return "Mute";
        case PunishmentType.Kick:
            return "Kick";
        case PunishmentType.Ban:
            if (modcase.punishedUntil) {
                return "TempBan"
            }
            return "Ban";
        default:
            return "None";
    }
}
