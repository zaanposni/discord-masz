import type { ILicenseEntry } from "./ILicenseEntry";

export interface ILicenseInfo {
    npm: Map<string, ILicenseEntry>
    dotnet: Map<string, ILicenseEntry>
    python: Map<string, ILicenseEntry>
}