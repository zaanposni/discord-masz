import { AppealStatus } from "./AppealStatus";

export interface IModCaseFilter {
    userIds?: string[];
    since?: string;
    before?: string;
    edited?: boolean;
    appealStatus?: AppealStatus[];
}
