import { ILanguageSelect } from "../models/ILanguageSelect";

export const APP_BASE_URL = '';
// export const APP_BASE_URL = 'http://127.0.0.1:5565';
export const API_URL = APP_BASE_URL + '/api/v1';
export const ENABLE_CORS = false;
// export const ENABLE_CORS = true;
export const DEFAULT_LANGUAGE = 'en';
export const LANGUAGES: ILanguageSelect[] = [
    {
        language: 'de',
        displayName: 'Deutsch'
    },
    {
        language: 'en',
        displayName: 'English'
    }
];