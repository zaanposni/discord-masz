<script lang="ts">
    import { Modal } from "carbon-components-svelte";
    import { showUserSettings } from "./store";
    import { Theme } from "carbon-components-svelte";
    import { LANGUAGES, LOCAL_STORAGE_KEY_THEME } from "../../config";
    import { currentTheme } from "../../stores/theme";
    import { Select, SelectItem } from "carbon-components-svelte";
    import { locale } from "svelte-i18n";
    import { currentLanguage } from "../../stores/currentLanguage";

    const themeSettings = {
        themes: ["white", "g10", "g90", "g100"],
        labelText: "Theme",
    };

    function onModalClose() {
        currentTheme.set(localStorage.getItem(LOCAL_STORAGE_KEY_THEME));
        showUserSettings.set(false);
    }

    function onLanguageSelect({detail}) {
        const language = LANGUAGES.find(l => l.language === detail);
        if (language) {
            locale.set(language.language);
            currentLanguage.set(language);
        }
    }
</script>

<Modal
    size="sm"
    open={$showUserSettings}
    modalHeading="Settings"
    primaryButtonText="Save"
    on:close={onModalClose}
    on:submit={onModalClose}>
    <Theme select={themeSettings} render="select" persist persistKey={LOCAL_STORAGE_KEY_THEME} />
    <Select class="mt-4" labelText="Language" on:change={onLanguageSelect}>
        {#each LANGUAGES as language}
            <SelectItem value={language.language} text={language.displayName} />
        {/each}
    </Select>
</Modal>
