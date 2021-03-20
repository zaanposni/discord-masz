import { GitHubAuthor } from "./GitHubAuthor";

export interface GitHubRelease {
    url: string;
    assets_url: string;
    upload_url: string;
    html_url: string;
    id: number;
    author: GitHubAuthor;
    node_id: string;
    tag_name: string;
    target_commitish: string;
    name: string;
    draft: boolean;
    prerelease: boolean;
    created_at: Date;
    published_at: Date;
    assets: any[];
    tarball_url: string;
    zipball_url: string;
    body: string;
}