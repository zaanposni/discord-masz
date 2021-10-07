export interface PatchNote {
    title: string;
    released_at: Date;
    changes: string[];
    contributors?: Contributor[];
    features: PatchNoteDetail[];
    fixes: PatchNoteDetail[];
    breaking: PatchNoteDetail[];
    technical: PatchNoteDetail[];
    gist_feature_link?: string;
    version: string;
}

export interface PatchNoteDetail {
    title: string;
    description: string;
    link?: string;
    icon?: string;
}

export interface Contributor {
    name: string;
    link: string;
    icon: string;
}
