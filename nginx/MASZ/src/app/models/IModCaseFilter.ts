import { PunishmentType } from "./PunishmentType";

export interface IModCaseFilter {
    customTextFilter?: string;
    userIds?: string[];
    moderatorIds?: string[];
    since?: string;
    before?: string;
    punishedUntilMin?: string;
    punishedUntilMax?: string;
    edited?: boolean;
    creationTypes?: number[];
    punishmentTypes?: PunishmentType[];
    punishmentActive?: boolean;
    lockedComments?: boolean;
    markedToDelete?: boolean;
}
