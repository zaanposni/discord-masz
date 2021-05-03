import { QuickSearchEntry } from "./QuickSearchEntry";
import { UserNoteView } from "./UserNoteView";

export interface QuickSearchResult {
    searchEntries: QuickSearchEntry[];
    userNoteView: UserNoteView;
    userMappings: any[];
}