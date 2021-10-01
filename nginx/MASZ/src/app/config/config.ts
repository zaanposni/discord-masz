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
        dateFormat: 'MMM d Y',
        dateTimeFormat: 'MMM d Y HH:mm:ss'
    },
    {
        language: 'de',
        displayName: 'German (deutsch)',
        apiValue: 1,
        dateFormat: 'd MMM Y',
        dateTimeFormat: 'd MMM Y HH:mm:ss'
    }
];