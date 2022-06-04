import type moment from "moment";

export interface IContributor {
    name: string;
    link: string;
    icon: string;
}

export interface IPatchnoteDetail {
    title: string;
    description: string;
    icon: string;
    link: string;
}

export interface IPatchnoteSeperator {
    text: string;
}

export interface IPatchnote {
    title: string;
    version: string;
    released_at: moment.Moment;
    contributors: IContributor[];
    features: IPatchnoteDetail[];
    fixes: IPatchnoteDetail[];
    technical: IPatchnoteDetail[];
    breaking: IPatchnoteDetail[];
    gist_feature_link: string;
    changes: string[];
}
