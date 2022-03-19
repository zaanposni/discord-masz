export const DEV_MODE = true;  // <-- USE THIS FOR DEVELOPMENT

export const APP_BASE_URL = DEV_MODE ? "http://127.0.0.1:5565" : "";
export const API_URL = APP_BASE_URL + '/api/v1';
export const ENABLE_CORS = DEV_MODE;

export const THEME_LOCAL_STORAGE_KEY = "__carbon-theme";

export const APP_NAME = "MASZ";
export const APP_VERSION = "v3.0";
