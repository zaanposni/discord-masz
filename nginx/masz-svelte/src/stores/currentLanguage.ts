import type { ILanguageSelect } from "../models/ILanguageSelect";
import type { Readable, Writable } from "svelte/store";
import { writable, derived } from "svelte/store";
import { LOCAL_STORAGE_KEY_LANGUAGE } from "../config";

let initial: boolean = true;
export const currentLanguage: Writable<ILanguageSelect> = writable(null);
currentLanguage.subscribe((language) => {
    if (language) {
        localStorage.setItem(LOCAL_STORAGE_KEY_LANGUAGE, language.language);
    } else {
        if (!initial) {
            localStorage.removeItem(LOCAL_STORAGE_KEY_LANGUAGE);
        }
        initial = false;
    }
});

const flatpickrImporter = async (locale) => {
    await import(`/i18n/${locale}.js`);
    return window.flatpickr.l10ns[locale];
}

export const currentFlatpickrLocale: Readable<any> = derived(
    currentLanguage,
    (language, set) => {
        const l = (language?.language ?? "en") === "en" ? "default" : language.language;
        flatpickrImporter(l).then((locale) => {
            set(locale);
        });
    },
    null
);
