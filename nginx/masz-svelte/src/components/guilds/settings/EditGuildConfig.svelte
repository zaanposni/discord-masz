<script lang="ts">
    import { _ } from "svelte-i18n";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import {
        Button,
        Checkbox,
        InlineLoading,
        Modal,
        MultiSelect,
        NumberInput,
        NumberInputSkeleton,
        Select,
        SelectItem,
        SelectSkeleton,
        SkeletonText,
        TextInput,
        TextInputSkeleton,
        Tooltip,
        Toggle,
    } from "carbon-components-svelte";
    import { derived, Writable } from "svelte/store";
    import { writable } from "svelte/store";
    import { currentParams } from "../../../stores/currentParams";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import { toastError, toastSuccess } from "../../../services/toast/store";
    import type { IGuildConfig } from "../../../models/api/IGuildConfig";
    import Language from "../../../services/enums/Language";
    import { goto } from "@roxi/routify";
    import { authUser } from "../../../stores/auth";
    import type { IAuthUser } from "../../../models/IAuthUser";
    import { confirmDialogMessageKey, confirmDialogReturnFunction, showConfirmDialog } from "../../../core/confirmDialog/store";

    export let addMode = false;
    export let guildId: string;

    const loading: Writable<boolean> = writable(!addMode);
    const submitting: Writable<boolean> = writable(!addMode);
    const data: Writable<IGuildConfig> = writable(
        addMode
            ? ({
                  guildId,
                  modRoles: [],
                  adminRoles: [],
                  mutedRoles: [],
                  allowBanAppealAfterDays: 0,
                  preferredLanguage: 0,
              } as any)
            : null
    );
    const webhookRegex = new RegExp("^https://discord(app)?.com/api/webhooks/.*$");

    const publicWebhookInvalid = derived(data, ($data) => {
        if (!$data) return true;
        return $data.modPublicNotificationWebhook && !webhookRegex.test($data.modPublicNotificationWebhook);
    });
    const internalWebhookInvalid = derived(data, ($data) => {
        if (!$data) return true;
        return $data.modInternalNotificationWebhook && !webhookRegex.test($data.modInternalNotificationWebhook);
    });

    const openDeleteDialog = writable(false);
    const confirmDelete = writable(false);
    const deleteAllResources = writable(false);

    let roles;

    if (addMode) {
        API.get(`/discord/guilds/${guildId}`, CacheMode.API_ONLY, false).then((res) => {
            roles = res.roles
                .filter((g) => g.id != guildId)
                .sort((a, b) => (a.position < b.position ? 1 : -1))
                .map((role) => ({
                    id: role.id,
                    text: role.name,
                }));
        });
    }

    $: $currentParams?.guildId ? loadGuildData() : null;
    function loadGuildData() {
        loading.set(true);
        submitting.set(false);
        data.set(null);

        API.get(`/guilds/${$currentParams.guildId}`, CacheMode.PREFER_CACHE, true).then((response: IGuildConfig) => {
            data.set({
                ...response,
                preferredLanguage: response.preferredLanguage.toString(),
            });
            loading.set(false);
        });

        roles = ($currentParams?.guild?.roles ?? [])
            .filter((g) => g.id != $currentParams.guildId)
            .sort((a, b) => (a.position < b.position ? 1 : -1))
            .map((role) => ({
                id: role.id,
                text: role.name,
            }));
    }

    function clearCache() {
        API.clearCacheEntry("get", `/guilds/${$currentParams.guildId}`);
    }

    function submit() {
        submitting.set(true);
        const dto: IGuildConfig = {
            ...$data,
            modInternalNotificationWebhook: $data.modInternalNotificationWebhook?.trim() != "" ? $data.modInternalNotificationWebhook : null,
            modPublicNotificationWebhook: $data.modPublicNotificationWebhook?.trim() != "" ? $data.modPublicNotificationWebhook : null,
        };
        if (addMode) {
            API.post(`/guilds`, dto, CacheMode.API_ONLY, false)
                .then(() => {
                    toastSuccess($_("guilds.config.saved"));
                    clearCache();
                    submitting.set(false);

                    API.get("discord/users/@me").then((res: IAuthUser) => {
                        authUser.set(res);
                        $goto(`/guilds/${guildId}`);
                    });
                })
                .catch((e) => {
                    toastError($_("guilds.config.failedtosave"));
                    submitting.set(false);
                });
        } else {
            API.put(`/guilds/${$currentParams.guildId}`, dto)
                .then(() => {
                    toastSuccess($_("guilds.config.saved"));
                    clearCache();
                    submitting.set(false);
                })
                .catch((e) => {
                    toastError($_("guilds.config.failedtosave"));
                    submitting.set(false);
                });
        }
    }

    function cleanupDeleteDialogs() {
        openDeleteDialog.set(false);
        deleteAllResources.set(false);
        showConfirmDialog.set(false);
        confirmDelete.set(false);
    }

    function showDeleteConfirm() {
        const deleteStore = $deleteAllResources;
        openDeleteDialog.set(false);
        setTimeout(() => {
            confirmDialogMessageKey.set("guilds.config.deleteguild");
            showConfirmDialog.set(true);
            confirmDialogReturnFunction.set((confirmed) => {
                if (confirmed) {
                    API.deleteData(`/guilds/${$currentParams.guildId}?deleteData=${deleteStore}`, {})
                        .then(() => {
                            submitting.set(true);
                            toastSuccess($_("guilds.config.deleted"));
                            API.get("discord/users/@me").then((res: IAuthUser) => {
                                authUser.set(res);
                                API.clearCache(true);
                                $goto(`/guilds`);
                            });
                        })
                        .catch((e) => {
                            console.error(e);
                            toastError($_("guilds.config.failedtodelete"));
                        })
                        .finally(() => {
                            cleanupDeleteDialogs();
                        });
                } else {
                    cleanupDeleteDialogs();
                }
            });
        }, 300);
    }
</script>

<Modal
    size="sm"
    danger
    open={$openDeleteDialog}
    selectorPrimaryFocus="#usernotememberselection"
    modalHeading={$_("guilds.config.deleteguild")}
    primaryButtonDisabled={!$confirmDelete}
    primaryButtonText={$_("guilds.config.deleteguild")}
    secondaryButtonText={$_("dialog.confirm.cancel")}
    on:close={cleanupDeleteDialogs}
    on:click:button--secondary={cleanupDeleteDialogs}
    on:submit={showDeleteConfirm}>
    <div class="text-lg font-bold text-red-600 mb-2">{$_("guilds.config.deleteundone")}</div>
    <Toggle class="mb-4" labelText={$_("guilds.config.deleteconfirm")} bind:toggled={$confirmDelete} />
    <Checkbox bind:checked={$deleteAllResources} labelText={$_("guilds.config.deleteresources")} />
</Modal>

<MediaQuery query="(min-width: 1024px)" let:matches>
    <!-- Header -->
    <div class="flex flex-col {matches ? 'w-1/2' : ''}">
        <div class="text-3xl font-bold mb-4">
            {$_("guilds.config.title")}
        </div>
    </div>

    {#if $loading}
        <div class="mt-4 {matches ? 'w-1/2' : ''}">
            <div class="grid grid-cols-1 lg:grid-cols-2 lg:gap-x-4 gap-y-2 items-center">
                <div class="lg:col-span-2">
                    <div class="flex flex-col">
                        <div class="text-lg font-bold mb-2">
                            <SkeletonText />
                        </div>
                        <div class="text-sm">
                            <SkeletonText />
                        </div>
                    </div>
                </div>
                <div>
                    <SelectSkeleton />
                </div>
                <div>
                    <SelectSkeleton />
                </div>
                <div>
                    <NumberInputSkeleton />
                </div>
                {#if matches}
                    <div />
                {/if}
                <div class="flex flex-row items-center">
                    <div class="shrink-0">
                        <Checkbox skeleton />
                    </div>
                    <div class="grow" />
                </div>
                {#if matches}
                    <div />
                {/if}

                <!-- ########################################################################### -->

                <div class="lg:col-span-2 mt-4">
                    <div class="flex flex-col">
                        <div class="text-lg font-bold mb-2">
                            <SkeletonText />
                        </div>
                        <div class="text-sm">
                            <SkeletonText />
                        </div>
                        <div class="text-sm">
                            <SkeletonText />
                        </div>
                    </div>
                </div>
                <div>
                    <SelectSkeleton />
                </div>
                {#if matches}
                    <div />
                {/if}

                <!-- ########################################################################### -->

                <div class="lg:col-span-2 mt-4">
                    <div class="flex flex-col">
                        <div class="text-lg font-bold mb-2">
                            <SkeletonText />
                        </div>
                        <div class="text-sm">
                            <SkeletonText />
                        </div>
                    </div>
                </div>

                <div class="lg:col-span-2">
                    <TextInputSkeleton />
                </div>
                <div class="lg:col-span-2">
                    <TextInputSkeleton />
                </div>

                <!-- ########################################################################### -->

                <div class="lg:col-span-2 mt-4">
                    <div class="flex flex-col">
                        <div class="text-lg font-bold mb-2">
                            <SkeletonText />
                        </div>
                    </div>
                </div>

                <div>
                    <div class="flex flex-row items-center">
                        <div class="shrink-0">
                            <Checkbox skeleton />
                        </div>
                        <div class="grow" />
                    </div>
                </div>
                {#if matches}
                    <div />
                {/if}
                <div>
                    <div class="flex flex-row items-center">
                        <div class="shrink-0">
                            <Checkbox skeleton />
                        </div>
                        <div class="grow" />
                    </div>
                </div>
                {#if matches}
                    <div />
                {/if}

                <!-- ########################################################################### -->

                <div class="lg:col-span-2 mt-4">
                    <div class="flex flex-col">
                        <div class="text-lg font-bold mb-2">
                            <SkeletonText />
                        </div>
                    </div>
                </div>

                <div>
                    <div class="flex flex-row items-center">
                        <div class="shrink-0">
                            <Checkbox skeleton />
                        </div>
                        <div class="grow" />
                    </div>
                </div>
                {#if matches}
                    <div />
                {/if}

                <div>
                    <SelectSkeleton />
                </div>
                {#if matches}
                    <div />
                {/if}
            </div>

            <!-- ########################################################################### -->

            <div class="mt-4 flex flex-row justify-end">
                <div>
                    <Button skeleton />
                </div>
            </div>
        </div>
    {:else if $data}
        <div class="mt-4 {matches ? 'w-1/2' : ''}">
            <div class="grid grid-cols-1 lg:grid-cols-2 lg:gap-x-4 gap-y-2 items-center">
                <div class="lg:col-span-2">
                    <div class="flex flex-col">
                        <div class="text-lg font-bold mb-2">
                            {$_("guilds.config.permissions.title")}
                        </div>
                        <div class="text-sm">
                            {$_("guilds.config.permissions.explain")}
                        </div>
                    </div>
                </div>
                <div>
                    <MultiSelect
                        disabled={$submitting}
                        titleText={$_("guilds.config.permissions.adminroles")}
                        items={roles}
                        bind:selectedIds={$data.adminRoles} />
                </div>
                <div>
                    <MultiSelect
                        disabled={$submitting}
                        titleText={$_("guilds.config.permissions.modroles")}
                        items={roles}
                        bind:selectedIds={$data.modRoles} />
                </div>
                <div>
                    <NumberInput
                        disabled={$submitting}
                        label={$_("guilds.config.permissions.appealafterxdays")}
                        bind:value={$data.allowBanAppealAfterDays}
                        min={0} />
                </div>
                {#if matches}
                    <div />
                {/if}
                <div class="flex flex-row items-center">
                    <div class="shrink-0">
                        <Checkbox
                            disabled={$submitting}
                            labelText={$_("guilds.config.permissions.strictpermissions")}
                            bind:checked={$data.strictModPermissionCheck} />
                    </div>
                    <div class="grow" />
                    <div class="shrink-0">
                        <Tooltip>
                            <p>{$_("guilds.config.permissions.strictpermissionsexplained")}</p>
                        </Tooltip>
                    </div>
                </div>
                {#if matches}
                    <div />
                {/if}

                <!-- ########################################################################### -->

                <div class="lg:col-span-2 mt-4">
                    <div class="flex flex-col">
                        <div class="text-lg font-bold mb-2">
                            {$_("guilds.config.mutedroles.title")}
                        </div>
                        <div class="text-sm">
                            {$_("guilds.config.mutedroles.explain")}
                        </div>
                        <div class="text-sm">
                            {$_("guilds.config.mutedroles.explain2")}
                        </div>
                    </div>
                </div>
                <div>
                    <MultiSelect
                        disabled={$submitting}
                        titleText={$_("guilds.config.mutedroles.title")}
                        items={roles}
                        bind:selectedIds={$data.mutedRoles} />
                </div>
                {#if matches}
                    <div />
                {/if}

                <!-- ########################################################################### -->

                <div class="lg:col-span-2 mt-4">
                    <div class="flex flex-col">
                        <div class="text-lg font-bold mb-2">
                            {$_("guilds.config.notifications.title")}
                        </div>
                        <div class="text-sm">
                            {$_("guilds.config.notifications.explain")}
                        </div>
                    </div>
                </div>

                <div class="lg:col-span-2">
                    <TextInput
                        disabled={$submitting}
                        invalid={$internalWebhookInvalid}
                        invalidText={$_("guilds.config.notifications.invalid")}
                        labelText={$_("guilds.config.notifications.internal")}
                        bind:value={$data.modInternalNotificationWebhook} />
                </div>
                <div class="lg:col-span-2">
                    <TextInput
                        disabled={$submitting}
                        invalid={$publicWebhookInvalid}
                        invalidText={$_("guilds.config.notifications.invalid")}
                        labelText={$_("guilds.config.notifications.public")}
                        bind:value={$data.modPublicNotificationWebhook} />
                </div>

                <!-- ########################################################################### -->

                <div class="lg:col-span-2 mt-4">
                    <div class="flex flex-col">
                        <div class="text-lg font-bold mb-2">
                            {$_("guilds.config.information.title")}
                        </div>
                    </div>
                </div>

                <div>
                    <div class="flex flex-row items-center">
                        <div class="shrink-0">
                            <Checkbox
                                disabled={$submitting}
                                labelText={$_("guilds.config.information.publicembedmode")}
                                bind:checked={$data.publicEmbedMode} />
                        </div>
                        <div class="grow" />
                        <div class="shrink-0">
                            <Tooltip>
                                <p>{$_("guilds.config.information.publicembedmodeexplained")}</p>
                            </Tooltip>
                        </div>
                    </div>
                </div>
                {#if matches}
                    <div />
                {/if}
                <div>
                    <div class="flex flex-row items-center">
                        <div class="shrink-0">
                            <Checkbox
                                disabled={$submitting}
                                labelText={$_("guilds.config.information.publishmod")}
                                bind:checked={$data.publishModeratorInfo} />
                        </div>
                        <div class="grow" />
                        <div class="shrink-0">
                            <Tooltip>
                                <p>{$_("guilds.config.information.publishmodexplained")}</p>
                            </Tooltip>
                        </div>
                    </div>
                </div>
                {#if matches}
                    <div />
                {/if}

                <!-- ########################################################################### -->

                <div class="lg:col-span-2 mt-4">
                    <div class="flex flex-col">
                        <div class="text-lg font-bold mb-2">
                            {$_("guilds.config.misc.title")}
                        </div>
                    </div>
                </div>

                <div>
                    <div class="flex flex-row items-center">
                        <div class="shrink-0">
                            <Checkbox disabled={$submitting} labelText={$_("guilds.config.misc.whois")} bind:checked={$data.executeWhoisOnJoin} />
                        </div>
                        <div class="grow" />
                        <div class="shrink-0">
                            <Tooltip>
                                <p>{$_("guilds.config.misc.whoisexplained")}</p>
                            </Tooltip>
                        </div>
                    </div>
                </div>
                {#if matches}
                    <div />
                {/if}

                <div>
                    <Select disabled={$submitting} labelText={$_("guilds.config.misc.language")} bind:selected={$data.preferredLanguage}>
                        {#each Language.getAll() as language}
                            <SelectItem value={language.id.toString()} text={$_(language.translationKey)} />
                        {/each}
                    </Select>
                </div>
                {#if matches}
                    <div />
                {/if}
            </div>

            <!-- ########################################################################### -->

            <div class="mt-4 flex flex-row justify-end">
                <div>
                    {#if !addMode && $authUser?.isAdmin}
                        <Button
                            on:click={() => {
                                openDeleteDialog.set(true);
                            }}
                            class="mr-2"
                            kind="danger"
                            disabled={$submitting}>
                            {$_("guilds.config.deleteguild")}</Button
                        >
                    {/if}
                    {#if $submitting}
                        <InlineLoading status="active" description={$_("guilds.config.submitting")} />
                    {:else}
                        <Button
                            on:click={submit}
                            disabled={$submitting ||
                                $internalWebhookInvalid ||
                                $publicWebhookInvalid ||
                                $data.modRoles.length === 0 ||
                                $data.adminRoles.length === 0}>
                            {$_("guilds.config.save")}</Button>
                    {/if}
                </div>
            </div>
        </div>
    {:else}
        :/
    {/if}
</MediaQuery>
