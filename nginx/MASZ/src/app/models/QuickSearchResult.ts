import { QuickSearchEntry } from "./QuickSearchEntry";
import { UserMappingView } from "./UserMappingView";
import { UserNoteView } from "./UserNoteView";

export interface QuickSearchResult {
    searchEntries: QuickSearchEntry[];
    userNoteView: UserNoteView;
    userMappingViews: UserMappingView[];
}