import { ILanguageSelect } from "../models/ILanguageSelect";

export const APP_BASE_URL = '';
// export const APP_BASE_URL = 'http://127.0.0.1:5565';
export const API_URL = APP_BASE_URL + '/api/v1';
export const ENABLE_CORS = false;
// export const ENABLE_CORS = true;
export const DEFAULT_LANGUAGE = 'en';
export const LANGUAGES: ILanguageSelect[] = [
    {
        language: 'en',
        displayName: 'English',
        apiValue: 0,
        dateFormat: 'MMM DD Y',
        dateTimeFormat: 'MMM DD Y HH:mm:ss'
    },
    {
        language: 'de',
        displayName: 'German (deutsch)',
        apiValue: 1,
        dateFormat: 'DD MMM Y',
        dateTimeFormat: 'DD MMM Y HH:mm:ss'
    },
    {
        language: 'fr',
        displayName: 'French (français)',
        apiValue: 2,
        dateFormat: 'DD MMM Y',
        dateTimeFormat: 'DD MMM Y HH:mm:ss'
    },
    {
        language: 'es',
        displayName: 'Spanish (español)',
        apiValue: 3,
        dateFormat: 'DD MMM Y',
        dateTimeFormat: 'DD MMM Y HH:mm:ss'
    },
    {
        language: 'it',
        displayName: 'Italian (italiano)',
        apiValue: 4,
        dateFormat: 'DD MMM Y',
        dateTimeFormat: 'DD MMM Y HH:mm:ss'
    },
    {
        language: 'at',
        displayName: 'Austrian (Österreichisch)',
        apiValue: 5,
        dateFormat: 'DD MMM Y',
        dateTimeFormat: 'DD MMM Y HH:mm:ss'
    },
    {
        language: 'ru',
        displayName: 'Russian (русский)',
        apiValue: 6,
        dateFormat: 'DD MMM Y',
        dateTimeFormat: 'DD MMM Y HH:mm:ss'
    }
];
export const DEFAULT_TIMEZONE = 'UTC';
export const TIMEZONES: string[] = [
    "-12:00",
    "-11:00",
    "-10:00",
    "-09:00",
    "-08:00",
    "-07:00",
    "-06:00",
    "-05:00",
    "-04:00",
    "-03:30",
    "-03:00",
    "-02:00",
    "-01:00",
    "UTC",
    "+01:00",
    "+02:00",
    "+03:00",
    "+03:30",
    "+04:00",
    "+04:30",
    "+05:00",
    "+05:30",
    "+05:45",
    "+06:00",
    "+06:30",
    "+07:00",
    "+08:00",
    "+08:45",
    "+09:00",
    "+09:30",
    "+10:00",
    "+10:30",
    "+11:00",
    "+12:00",
    "+12:45",
    "+13:00",
    "+14:00"
]

