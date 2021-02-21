import { AutoModerationEventTableEntry } from "./AutoModerationEventTableEntry";
import { ModCaseTable } from "./ModCaseTable";

export interface QuickSearchEntry {
    entry: ModCaseTable|AutoModerationEventTableEntry;
    createdAt: Date;
    quickSearchEntryType: number;
}