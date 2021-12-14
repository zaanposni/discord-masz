import { PunishmentType } from "./PunishmentType";

export interface IModCaseFilter {
    customTextFilter?: string;
    userIds?: string[];
    modIds?: string[];
    since?: Date;
    before?: Date;
    punishedUntilMin?: Date;
    punishedUntilMax?: Date;
    edited?: boolean;
    creationTypes?: number[];
    punishmentTypes?: PunishmentType[];
    punishmentActive?: boolean;
    lockedCommentes?: boolean;
    markedToDelete?: boolean;
}
