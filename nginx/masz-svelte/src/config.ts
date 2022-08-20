import type { ILanguageSelect } from "./models/ILanguageSelect";

export const DEV_MODE = false;  // <-- USE THIS FOR DEVELOPMENT

export const APP_BASE_URL = DEV_MODE ? "http://127.0.0.1:5565" : "";
export const API_URL = APP_BASE_URL + '/api/v1';
export const ENABLE_CORS = DEV_MODE;

export const LOCAL_STORAGE_KEY_THEME = "__carbon-theme";
export const LOCAL_STORAGE_KEY_LANGUAGE = "__masz-language";
export const LOCAL_STORAGE_KEY_FAVORITE_GUILD = "__masz-favorite-guild";
export const LOCAL_STORAGE_KEY_ADMIN_DASHBOARD_ITEMS = "__masz-admin-dashboard-items";
export const LOCAL_STORAGE_KEY_GUILD_DASHBOARD_ITEMS = "__masz-guild-dashboard-items";
export const LOCAL_STORAGE_KEY_GUILD_QUICKSEARCH_HISTORY = "__masz-guild-quicksearch-history";

export const FEEDBACK_COOKIE_NAME = "__masz-feedback";

export const APP_NAME = "MASZ";
export const APP_VERSION = "v3.1.3";

export const GUILD_QUICKSEARCH_MAX_HISTORY_ENTRIES = 10;

export const API_CACHE_ENABLE = !DEV_MODE;
export const API_CACHE_LIFETIME = 1000 * 60 * 60;
export const API_CACHE_CLEAR_LIFETIME = 1000 * 60;

export const LANGUAGES: ILanguageSelect[] = [
    {
        language: "en",
        displayName: "English",
        dateFormat: "m/d/Y",
        momentDateFormat: "MM/DD/YYYY",
        timeFormat: "hh:MM",
        momentTimeFormat: "HH:mm",
        momentDateTimeFormat: "MM/DD/YYYY HH:mm",
        apiValue: 0,
    },
    {
        language: "de",
        displayName: "German (deutsch)",
        dateFormat: "d.m.Y",
        momentDateFormat: "DD.MM.YYYY",
        timeFormat: "hh:MM",
        momentTimeFormat: "HH:mm",
        momentDateTimeFormat: "DD.MM.YYYY HH:mm",
        apiValue: 1,
    },
    {
        language: "fr",
        displayName: "French (français)",
        dateFormat: "d/m/Y",
        momentDateFormat: "DD/MM/YYYY",
        timeFormat: "hh:MM",
        momentTimeFormat: "HH:mm",
        momentDateTimeFormat: "DD/MM/YYYY HH:mm",
        apiValue: 2,
    },
    {
        language: "es",
        displayName: "Spanish (español)",
        dateFormat: "d/m/Y",
        momentDateFormat: "DD/MM/YYYY",
        timeFormat: "hh:MM",
        momentTimeFormat: "HH:mm",
        momentDateTimeFormat: "DD/MM/YYYY HH:mm",
        apiValue: 3,
    },
    {
        language: "it",
        displayName: "Italian (italiano)",
        dateFormat: "d/m/Y",
        momentDateFormat: "DD/MM/YYYY",
        timeFormat: "hh:MM",
        momentTimeFormat: "HH:mm",
        momentDateTimeFormat: "DD/MM/YYYY HH:mm",
        apiValue: 4,
    },
    {
        language: "at",
        displayName: "Austrian (Österreichisch)",
        dateFormat: "d.m.Y",
        momentDateFormat: "DD.MM.YYYY",
        timeFormat: "hh:MM",
        momentTimeFormat: "HH:mm",
        momentDateTimeFormat: "DD.MM.YYYY HH:mm",
        apiValue: 5,
    },
    {
        language: "ru",
        displayName: "Russian (русский)",
        dateFormat: "d.m.Y",
        momentDateFormat: "DD.MM.YYYY",
        timeFormat: "hh:MM",
        momentTimeFormat: "HH:mm",
        momentDateTimeFormat: "DD.MM.YYYY HH:mm",
        apiValue: 6,
    },
];
