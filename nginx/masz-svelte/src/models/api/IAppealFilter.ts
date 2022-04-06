import type { AppealStatus } from "./AppealStatus";

export interface IAppealFilter {
    userIds?: string[];
    since?: string;
    before?: string;
    status?: AppealStatus[];
}
