import { APIEnum } from "./APIEnum";
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

export function convertModcaseToPunishmentString(modcase?: ModCase, punishments?: APIEnum[]): string {
    return punishments?.find(x => x.key === modcase?.punishmentType)?.value ?? "Unknown";
}
