import { ModCase } from "./ModCase";

export interface CaseDeleteDialogData {
    case: ModCase;
    sendNotification: boolean;
    isAdmin: boolean;
    forceDelete: boolean;
}