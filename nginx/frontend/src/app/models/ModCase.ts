import { CaseComment } from "./CaseComment";

export interface ModCase {
    id: number;
    caseId: number;
    guildId: string;
    title: string;
    description: string;
    userId: string;
    username: string;
    nickname?: string;
    modId: string;
    severity: number;
    createdAt: Date;
    occuredAt: Date;
    lastEditedAt: Date;
    lastEditedByModId: string;
    punishment: string;
    labels: string[];
    others?: string;
    valid: boolean;
    punishmentType: number;
    punishedUntil: Date;
    punishmentActive: boolean;
    comments: CaseComment[];
}