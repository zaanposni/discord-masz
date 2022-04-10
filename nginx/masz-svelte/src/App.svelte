<script lang="ts">
    import { LOCAL_STORAGE_KEY_LANGUAGE, LANGUAGES } from "./config";
    import { Router } from "@roxi/routify";
    import { routes } from "../.routify/routes";
    import "carbon-components-svelte/css/all.css";
    import Toasts from "./services/toast/toasts.svelte";
    import { currentTheme } from "./stores/theme";
    import API from "./services/api/api";
    import { register, init as initI18N, getLocaleFromNavigator, isLoading, locale } from "svelte-i18n";
    import { currentLanguage } from "./stores/currentLanguage";
    import { applicationInfo } from "./stores/applicationInfo";
    import { CacheMode } from "./services/api/CacheMode";
    import moment from "moment";
    import 'moment/min/locales';
    
    register("en", () => {
        return API.getAsset("/i18n/en.json");
    });
    register("en-us", () => {
        return API.getAsset("/i18n/en.json");
    });
    register("de", () => {
        return API.getAsset("/i18n/de.json");
    });
    register("at", () => {
        return API.getAsset("/i18n/at.json");
    });
    register("es", () => {
        return API.getAsset("/i18n/es.json");
    });
    register("fr", () => {
        return API.getAsset("/i18n/fr.json");
    });
    register("it", () => {
        return API.getAsset("/i18n/it.json");
    });
    register("ru", () => {
        return API.getAsset("/i18n/ru.json");
    });

    setTimeout(() => {
        const html = document.getElementsByTagName("html");
        if (html?.length != 0) {
            const htmlNode = html[0];
            if (htmlNode) {
                currentTheme.set(htmlNode.getAttribute("theme"));
            }
        }
    }, 100);

    setTimeout(() => {
        const languageLocalStorage = localStorage.getItem(LOCAL_STORAGE_KEY_LANGUAGE);
        let language;
        if (languageLocalStorage) {
            locale.set(languageLocalStorage);
            moment.locale(languageLocalStorage);
            language = LANGUAGES.find((l) => l.language === languageLocalStorage);
        } else {
            language = LANGUAGES.find((l) => l.language === $locale);
        }
        if (language) {
            currentLanguage.set(language);
        }
    }, 100);

    initI18N({
        fallbackLocale: "en",
        initialLocale: localStorage.getItem(LOCAL_STORAGE_KEY_LANGUAGE) || getLocaleFromNavigator() || "en",
    });

    API.get("/meta/application", CacheMode.PREFER_CACHE, true).then((data) => {
        applicationInfo.set(data);
    });
</script>

<style global lang="postcss">
    @tailwind base;
    @tailwind components;
    @tailwind utilities;
</style>

<Toasts />
{#if $isLoading}
    Please wait...
{:else}
    <Router {routes} />
{/if}
