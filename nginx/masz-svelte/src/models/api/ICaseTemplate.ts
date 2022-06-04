import type moment from "moment";

export interface ICaseTemplate {
    id: number;
    userId: string;
    templateName: string;
    createdForGuildId: string;
    viewPermission: number;
    createdAt: moment.Moment;
    caseTitle: string;
    caseDescription: string;
    caseLabels: string[];
    casePunishmentType: number;
    casePunishedUntil?: moment.Moment;
    sendPublicNotification: boolean;
    handlePunishment: boolean;
    announceDm: boolean;
}
