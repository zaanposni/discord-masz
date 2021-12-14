import { PunishmentType } from "./PunishmentType";

export interface IModCaseFilter {
    customTextFilter?: string;
    userIds?: string[];
    moderatorIds?: string[];
    since?: Date;
    before?: Date;
    punishedUntilMin?: Date;
    punishedUntilMax?: Date;
    edited?: boolean;
    creationTypes?: number[];
    punishmentTypes?: PunishmentType[];
    punishmentActive?: boolean;
    lockedComments?: boolean;
    markedToDelete?: boolean;
}
