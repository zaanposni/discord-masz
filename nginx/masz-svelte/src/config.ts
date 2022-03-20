import type { ILanguageSelect } from "./models/ILanguageSelect";

export const DEV_MODE = true;  // <-- USE THIS FOR DEVELOPMENT

export const APP_BASE_URL = DEV_MODE ? "http://127.0.0.1:5565" : "";
export const API_URL = APP_BASE_URL + '/api/v1';
export const ENABLE_CORS = DEV_MODE;

export const THEME_LOCAL_STORAGE_KEY = "__carbon-theme";
export const LOCAL_STORAGE_KEY_LANGUAGE = "__masz-language";

export const APP_NAME = "MASZ";
export const APP_VERSION = "v3.0";

export const LANGUAGES: ILanguageSelect[] = [
    {
        language: "en",
        displayName: "English",
        apiValue: 0,
    },
    {
        language: "de",
        displayName: "German (deutsch)",
        apiValue: 1,
    },
    {
        language: "fr",
        displayName: "French (français)",
        apiValue: 2,
    },
    {
        language: "es",
        displayName: "Spanish (español)",
        apiValue: 3,
    },
    {
        language: "it",
        displayName: "Italian (italiano)",
        apiValue: 4,
    },
    {
        language: "at",
        displayName: "Austrian (Österreichisch)",
        apiValue: 5,
    },
    {
        language: "ru",
        displayName: "Russian (русский)",
        apiValue: 6,
    },
];
