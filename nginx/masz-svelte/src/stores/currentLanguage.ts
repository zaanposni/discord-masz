import type { ILanguageSelect } from "../models/ILanguageSelect";
import type { Writable } from "svelte/store";
import { writable } from "svelte/store";
import { LOCAL_STORAGE_KEY_LANGUAGE } from "../config";

export const currentLanguage: Writable<ILanguageSelect> = writable(null);
currentLanguage.subscribe(language => {
    if (language) {
        localStorage.setItem(LOCAL_STORAGE_KEY_LANGUAGE, language.language);
    } else {
        localStorage.removeItem(LOCAL_STORAGE_KEY_LANGUAGE);
    }
});
