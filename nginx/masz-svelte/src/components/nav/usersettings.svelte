<script lang="ts">
    import { Modal } from "carbon-components-svelte";
    import { showUserSettings } from "./store";
    import { Theme } from "carbon-components-svelte";
    import { LOCAL_STORAGE_KEY_THEME } from "../../config";
    import { currentTheme } from "../../stores/theme";

    const themeSettings = {
        themes: ["white", "g10", "g90", "g100"],
        labelText: "Theme",
    };

    function onModalClose() {
        currentTheme.set(localStorage.getItem(LOCAL_STORAGE_KEY_THEME));
        showUserSettings.set(false);
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
</Modal>
