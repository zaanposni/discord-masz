export enum AutoModerationAction {
    None,
    ContentDeleted,
    CaseCreated,
    ContentDeletedAndCaseCreated
}

export function AutoModerationActionOptions() : Array<string> {
    let keys = Object.keys(AutoModerationAction);
    return keys.slice(keys.length / 2);
}
