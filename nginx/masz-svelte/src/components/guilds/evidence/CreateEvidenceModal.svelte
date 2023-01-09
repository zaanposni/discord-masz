<script lang="ts">
    import { TextInput, Button, Modal } from "carbon-components-svelte";
    import { createEventDispatcher } from "svelte";
    import { writable } from "svelte/store";
    import { _ } from "svelte-i18n";
    import { toastError, toastSuccess } from "../../../services/toast/store";
    import { currentParams } from "../../../stores/currentParams";
    import API from "../../../services/api/api";

    export let open = false;

    const dispatcher = createEventDispatcher();
    const createModalContent = writable("");
    const createModalSubmitting = writable(false);
    const createRegex = new RegExp(/(https?:\/\/)?([a-zA-Z0-9]+.)?discord\.com\/channels\/(\d+)\/(\d+)\/(\d+)/)

    function onModalClose() {
        open = false;
        setTimeout(() => {
            createModalSubmitting.set(false);
            createModalContent.set("");
        }, 200);
    }

    function onCreate() {
        let m: RegExpExecArray;
        if((m = createRegex.exec($createModalContent)) == null) {
            toastError($_("guilds.evidencetable.badcreate"))
            return;
        }
        const guildId = m[3];
        const channelId = m[4];
        const messageId = m[5];

        if(guildId !== $currentParams.guildId) {
            toastError($_("guilds.evidencetable.badguild"))
            return;
        }

        createModalSubmitting.set(true);

        const data = {
            channelId: channelId,
            messageId: messageId,
        }

        API.post(`guilds/${guildId}/evidence`, data)
            .then((res) => {
                dispatcher("create", res);
                toastSuccess($_("guilds.evidencetable.created"))
                onModalClose();
            })
            .catch((err) => {
                console.error(err);
                toastError($_("guilds.evidencetable.createerror"))
            });
    }
</script>

<Modal
    size="sm"
    {open}
    primaryButtonText={$_("guilds.evidencetable.create")}
    secondaryButtonText={$_("dialog.confirm.cancel")}
    modalHeading={$_("guilds.evidencetable.create")}
    selectorPrimaryFocus="#createModalContent"
    primaryButtonDisabled={$createModalContent === "" || $createModalSubmitting}
    on:close={onModalClose}
    on:click:button--secondary={onModalClose}
    on:submit={onCreate}>
    <div class="mb-2">
        <TextInput
            id="createModalContent"
            disabled={$createModalSubmitting}
            labelText={$_("guilds.evidencetable.createinstruction")}
            placeholder={$_("guilds.evidencetable.createplaceholder")}
            bind:value={$createModalContent} />
    </div>
</Modal>
