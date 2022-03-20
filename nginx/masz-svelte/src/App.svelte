<script lang="ts">
    import { LOCAL_STORAGE_KEY_LANGUAGE, LANGUAGES } from "./config";
    import { Router } from "@roxi/routify";
    import { routes } from "../.routify/routes";
    import "carbon-components-svelte/css/all.css";
    import Toasts from "./services/toast/toasts.svelte";
    import { currentTheme } from "./stores/theme";
    import API from "./services/api/api";

    import { register, init, getLocaleFromNavigator } from "svelte-i18n";
    import { currentLanguage } from "./stores/currentLanguage";
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
        currentTheme.set(document.getElementsByTagName("html")[0].getAttribute("theme"));
    }, 100);

    let initialLocale = getLocaleFromNavigator();
    if (localStorage.getItem(LOCAL_STORAGE_KEY_LANGUAGE)) {
        initialLocale = localStorage.getItem(LOCAL_STORAGE_KEY_LANGUAGE);
    } else {
        const language = LANGUAGES.find((l) => l.language === getLocaleFromNavigator());
        if (language) {
            currentLanguage.set(language);
        }
    }

    init({
        fallbackLocale: "en",
        initialLocale,
    });
</script>

<style global lang="postcss">
    @tailwind base;
    @tailwind components;
    @tailwind utilities;
</style>

<Toasts />
<Router {routes} />
