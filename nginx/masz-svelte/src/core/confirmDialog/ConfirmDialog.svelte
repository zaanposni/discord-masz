<script lang="ts">
    import { Modal } from "carbon-components-svelte";
    import { _ } from "svelte-i18n";
    import { confirmDialogMessageKey, confirmDialogReturnFunction, showConfirmDialog } from "./store";

    function onModalClose() {
        showConfirmDialog.set(false);
        confirmDialogMessageKey.set("dialog.confirm.message");
        if ($confirmDialogReturnFunction) {
            $confirmDialogReturnFunction(false);
        }
        confirmDialogReturnFunction.set(null);
    }

    function onModalSubmit() {
        showConfirmDialog.set(false);
        confirmDialogMessageKey.set("dialog.confirm.message");
        if ($confirmDialogReturnFunction) {
            $confirmDialogReturnFunction(true);
        }
        confirmDialogReturnFunction.set(null);
    }
</script>

<Modal
    size="sm"
    open={$showConfirmDialog}
    modalHeading={$_("dialog.confirm.title")}
    on:close={onModalClose}
    on:submit={onModalSubmit}
    on:click:button--secondary={onModalClose}
    primaryButtonText={$_("dialog.confirm.confirm")}
    secondaryButtonText={$_("dialog.confirm.cancel")}>
    {$_($confirmDialogMessageKey)}
</Modal>
