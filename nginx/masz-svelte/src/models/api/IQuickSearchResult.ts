import type { ICompactCaseView } from "./ICompactCaseView";
import type { IUserMappingView } from "./IUserMappingView";
import type { IUserNoteView } from "./IUserNoteView";

export interface IQuickSearchResult {
    cases: ICompactCaseView[];
    userNoteView: IUserNoteView[];
    userMappingViews: IUserMappingView[];
}