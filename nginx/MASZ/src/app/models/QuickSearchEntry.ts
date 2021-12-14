import { AutoModerationEventTableEntry } from "./AutoModerationEventTableEntry";
import { IModCaseTableEntry } from "./IModCaseTableEntry";

export interface QuickSearchEntry {
    entry: IModCaseTableEntry|AutoModerationEventTableEntry;
    createdAt: Date;
    quickSearchEntryType: number;
}