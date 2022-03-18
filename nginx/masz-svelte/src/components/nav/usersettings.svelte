<script lang="ts">
  import { Modal } from "carbon-components-svelte";
  import { showUserSettings } from "./store";
  import { Theme } from "carbon-components-svelte";
  import { THEME_LOCAL_STORAGE_KEY } from "../../config";
import { currentTheme } from "../../stores/theme";

  const themeSettings = {
    themes: ["white", "g10", "g80", "g90", "g100"],
    labelText: "Theme"
  };

  function onModalClose() {
    currentTheme.set(localStorage.getItem(THEME_LOCAL_STORAGE_KEY));
    showUserSettings.set(false);
  }
</script>

<Modal
  size="sm"
  open={$showUserSettings}
  modalHeading="Settings"
  primaryButtonText="Save"
  on:close={onModalClose}
  on:submit={onModalClose}
>
  <Theme select={themeSettings} render="select" persist persistKey={THEME_LOCAL_STORAGE_KEY} />
</Modal>
