<script lang="ts">
    import { Modal } from "carbon-components-svelte";
    import { showUserSettings } from "./store";
    import { Theme } from "carbon-components-svelte";
    import { LANGUAGES, LOCAL_STORAGE_KEY_LANGUAGE, LOCAL_STORAGE_KEY_THEME } from "../../config";
    import { currentTheme } from "../../stores/theme";
    import { Select, SelectItem } from "carbon-components-svelte";
    import { locale, _ } from "svelte-i18n";
    import { currentLanguage } from "../../stores/currentLanguage";
import moment from "moment";

    const themeSettings = {
        themes: ["white", "g10", "g90", "g100"],
        labelText: "Theme",
    };

    // default to g90
    if (localStorage.getItem(LOCAL_STORAGE_KEY_THEME) === null) {
        localStorage.setItem(LOCAL_STORAGE_KEY_THEME, "g90");
    }

    function onModalClose() {
        currentTheme.set(localStorage.getItem(LOCAL_STORAGE_KEY_THEME));
        showUserSettings.set(false);
    }

    let initial: boolean = true;
    let selectedLanguage = LANGUAGES?.find((l) => l.language === $locale)?.language || localStorage.getItem(LOCAL_STORAGE_KEY_LANGUAGE) || "en";
    function onLanguageSelect({ detail }) {
        if (!initial) {
            const language = LANGUAGES.find((l) => l.language === detail);
            if (language) {
                locale.set(language.language);
                moment.locale(language.language);
                currentLanguage.set(language);
            }
        }
        initial = false;
    }
</script>

<Modal
    size="sm"
    selectorPrimaryFocus="#initialfocus"
    open={$showUserSettings}
    passiveModal
    modalHeading="{$_('settings.base')}"
    on:close={onModalClose}
    on:submit={onModalClose}>
    <div id="initialfocus" class="mb-2">{$_('settings.autosave')}</div>
    <Theme select={themeSettings} render="select" persist persistKey={LOCAL_STORAGE_KEY_THEME} />
    <Select class="mt-2" labelText="{$_('settings.language')}" on:change={onLanguageSelect} selected={selectedLanguage}>
        {#each LANGUAGES as language}
            <SelectItem value={language.language} text={language.displayName} />
        {/each}
    </Select>
</Modal>
