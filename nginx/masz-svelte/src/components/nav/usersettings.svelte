<script lang="ts">
  import { Modal } from "carbon-components-svelte";
  import { showUserSettings } from "./store";
  import { Theme } from "carbon-components-svelte";

  const themeSettings = {
    themes: ["white", "g10", "g80", "g90", "g100"],
    labelText: "Theme"
  };
</script>

<Modal
  size="sm"
  open={$showUserSettings}
  modalHeading="Settings"
  primaryButtonText="Save"
  on:close={() => {
    showUserSettings.set(false);
  }}
  on:submit={() => {
    showUserSettings.set(false);
  }}
>
  <Theme select={themeSettings} render="select" persist persistKey="__carbon-theme"/>
</Modal>
