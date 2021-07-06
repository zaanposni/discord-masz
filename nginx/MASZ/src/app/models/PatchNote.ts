export interface PatchNote {
    title: string;
    released_at: Date;
    changes: string[];
    features: PatchNoteDetail[];
    fixes: PatchNoteDetail[];
    breaking: PatchNoteDetail[];
    version: string;
}

export interface PatchNoteDetail {
    title: string;
    description: string;
}
