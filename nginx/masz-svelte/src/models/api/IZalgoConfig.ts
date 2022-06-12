export interface IZalgoConfig {
    id: number;
    guildId: string;
    enabled: boolean;
    percentage: number;
    renameNormal: boolean;
    renameFallback: string;
    logToModChannel: boolean;
}