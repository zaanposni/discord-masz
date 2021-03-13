export interface CaseTemplate {
    id: number;
    userId: string;
    templateName: string;
    createdForGuildId: string;
    viewPermission: number;
    createdAt: Date;
    caseTitle: string;
    caseDescription: string;
    caseLabels: string[];
    casePunishment: string;
    casePunishmentType: number;
    casePunishedUntil?: Date;
    sendPublicNotification: boolean;
    handlePunishment: boolean;
    announceDm: boolean;
}