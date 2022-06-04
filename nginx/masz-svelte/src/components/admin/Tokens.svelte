<script lang="ts">
    import {
        Button,
        DataTable,
        Toolbar,
        ToolbarContent,
        ToolbarSearch,
        ToolbarMenu,
        ToolbarMenuItem,
        Modal,
        TextInput,
        InlineNotification,
    } from "carbon-components-svelte";
    import { Copy24, Information24, TrashCan24 } from "carbon-icons-svelte";
    import moment from "moment";
    import { _ } from "svelte-i18n";
    import type { Writable } from "svelte/store";
    import { writable } from "svelte/store";
    import type { IAPIToken } from "../../models/api/IAPIToken";
    import API from "../../services/api/api";
    import { CacheMode } from "../../services/api/CacheMode";
    import { toastError, toastSuccess } from "../../services/toast/store";
    import { currentLanguage } from "../../stores/currentLanguage";
    import { slide } from "svelte/transition";
    import MediaQuery from "../../core/MediaQuery.svelte";

    const tokens: Writable<IAPIToken[]> = writable([]);
    const newTokenName: Writable<string> = writable("");
    const newTokenModalOpen: Writable<boolean> = writable(false);
    const newTokenSubmitting: Writable<boolean> = writable(false);
    const newTokenRawValue: Writable<string> = writable("");

    let headers = [];
    $: headers = [
        { key: "name", value: $_("admin.tokens.name"), minWidth: "60px" },
        { key: "createdAt", value: $_("admin.tokens.createdat"), minWidth: "60px" },
        { key: "validUntil", value: $_("admin.tokens.validuntil"), minWidth: "60px" },
        { key: "action", value: $_("admin.tokens.action"), width: "120px" },
    ];

    function reloadData() {
        API.get("/token", CacheMode.API_ONLY, false)
            .then((response) => {
                tokens.set([response]);
            })
            .catch((error) => {
                console.error(error);
            });
    }
    reloadData();

    function deleteToken() {
        API.deleteData("/token", {})
            .then((res) => {
                tokens.set([]);
                toastSuccess($_("admin.tokens.deleted"));
            })
            .catch((err) => {
                toastError($_("admin.tokens.failedtodelete"));
            });
    }

    function createToken() {
        newTokenSubmitting.set(true);
        API.post("/token", {
            name: $newTokenName,
        })
            .then((res: { token: string }) => {
                newTokenRawValue.set(res.token);
                newTokenSubmitting.set(false);
                toastSuccess($_("admin.tokens.created"));
                reloadData();
            })
            .catch(() => {
                newTokenSubmitting.set(false);
                toastError($_("admin.tokens.failedtocreate"));
            });
    }

    function onModalReset() {
        newTokenModalOpen.set(false);
        setTimeout(() => {
            newTokenName.set("");
            newTokenRawValue.set("");
            newTokenSubmitting.set(false);
        }, 300);
    }

    function copyToken() {
        navigator.clipboard.writeText($newTokenRawValue).catch((e) => console.error(e));
    }
</script>

<Modal
    size="sm"
    bind:open={$newTokenModalOpen}
    modalHeading={$_("admin.tokens.createnew")}
    passiveModal={!$newTokenRawValue}
    primaryButtonText={$_("admin.tokens.copy")}
    primaryButtonIcon={Copy24}
    secondaryButtonText={$_("admin.tokens.cancel")}
    shouldSubmitOnEnter={false}
    on:click:button--primary={copyToken}
    on:click:button--secondary={onModalReset}
    on:close={onModalReset}
    on:submit>
    {#if !$newTokenRawValue}
        <div class="flex flex-col" transition:slide|local>
            <div class="mb-2">
                <InlineNotification kind="warning" subtitle={$_("admin.tokens.powerful")} />
            </div>
            <TextInput bind:value={$newTokenName} labelText={$_("admin.tokens.name")} disabled={$newTokenSubmitting} />
            <div class="flex flex-row items-end justify-end mt-2">
                <Button disabled={$newTokenSubmitting || $newTokenName.trim() === ""} on:click={createToken}>{$_("admin.tokens.generate")}</Button>
            </div>
        </div>
    {:else}
        <div class="flex flex-col" transition:slide|local>
            <TextInput value={$newTokenRawValue} readonly />
            <div class="flex flex-row flex-nowrap mt-2">
                <Information24 class="mr-2" />
                <div class="italic">{$_('admin.tokens.unique')}</div>
            </div>
        </div>
    {/if}
</Modal>

<MediaQuery query="(min-width: 1024px)" let:matches>
    <div class="w-full lg:w-1/2">
        {#if $tokens.length !== 0}
            <div>
                <InlineNotification kind="info" subtitle={$_("admin.tokens.onlyone")} />
            </div>
        {/if}
        <DataTable zebra title={$_("admin.tokens.title")} description={$_("admin.tokens.tabledescription")} {headers} bind:rows={$tokens}>
            <Toolbar>
                <ToolbarContent>
                    {#if matches && $tokens.length > 0}
                        <ToolbarSearch
                            persistent
                            shouldFilterRows={(row, value) => {
                                return row.name.toLowerCase().includes(value.toLowerCase());
                            }} />
                    {/if}
                    <ToolbarMenu>
                        <ToolbarMenuItem target="_blank" href="https://discord.gg/5zjpzw6h3S">{$_("admin.tokens.support")}</ToolbarMenuItem>
                        <ToolbarMenuItem
                            target="_blank"
                            href="https://github.com/zaanposni/discord-masz/blob/master/docs/discord-masz.postman_collection.json"
                            >{$_("admin.tokens.documentation")}</ToolbarMenuItem>
                        {#if $tokens.length > 0}
                            <ToolbarMenuItem on:click={deleteToken} hasDivider danger>{$_("admin.tokens.deleteall")}</ToolbarMenuItem>
                        {/if}
                    </ToolbarMenu>
                    <Button
                        on:click={() => {
                            newTokenModalOpen.set(true);
                        }}
                        disabled={$tokens.length !== 0}>{$_("admin.tokens.createnew")}</Button>
                </ToolbarContent>
            </Toolbar>
            <svelte:fragment slot="cell" let:row let:cell>
                {#if cell.key === "action"}
                    <div class="flex items-center justify-center">
                        <Button on:click={deleteToken} icon={TrashCan24} kind="danger" iconDescription={$_("admin.tokens.delete")} />
                    </div>
                {:else if cell.key === "createdAt" || cell.key === "validUntil"}
                    {moment(cell.value).format($currentLanguage?.momentDateFormat ?? "YYYY-MM-DD")}
                {:else}
                    {cell.value}
                {/if}
            </svelte:fragment>
        </DataTable>
    </div>
</MediaQuery>
