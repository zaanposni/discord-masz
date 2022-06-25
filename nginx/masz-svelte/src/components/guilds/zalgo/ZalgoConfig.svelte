<script lang="ts">
    import {
        Button,
        Checkbox,
        InlineLoading,
        Loading,
        Modal,
        NumberInput,
        NumberInputSkeleton,
        TextInput,
        TextInputSkeleton,
    } from "carbon-components-svelte";
    import { ArrowRight20 } from "carbon-icons-svelte";
    import Warning_02 from "carbon-pictograms-svelte/lib/Warning_02.svelte";
    import { _ } from "svelte-i18n";
    import { writable, Writable } from "svelte/store";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import type { IZalgoConfig } from "../../../models/api/IZalgoConfig";
    import type { IDiscordUser } from "../../../models/discord/IDiscordUser";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import { toastError, toastSuccess } from "../../../services/toast/store";
    import { currentParams } from "../../../stores/currentParams";
    import UserIcon from "../../discord/UserIcon.svelte";

    const loading: Writable<boolean> = writable(false);
    const submitting: Writable<boolean> = writable(false);
    const zalgo: Writable<IZalgoConfig> = writable(null);

    const showModal: Writable<boolean> = writable(false);
    const modalSubmitting: Writable<boolean> = writable(false);
    const simulateResult: Writable<{ oldName: string; newName: string; user: IDiscordUser }[]> = writable([]);

    $: $currentParams?.guildId ? loadGuildData() : null;
    function loadGuildData() {
        loading.set(true);
        zalgo.set(null);
        submitting.set(false);

        API.get(`/guilds/${$currentParams.guildId}/zalgo`, CacheMode.PREFER_CACHE, true)
            .then((response: IZalgoConfig) => {
                zalgo.set(response);
                loading.set(false);
            })
            .catch(() => {
                zalgo.set({
                    id: 0,
                    guildId: $currentParams.guildId,
                    enabled: false,
                    percentage: 0,
                    renameNormal: false,
                    renameFallback: "zalgo user",
                    logToModChannel: false,
                });
                loading.set(false);
            });
    }

    function clearCache() {
        API.clearCacheEntry("get", `/guilds/${$currentParams.guildId}/zalgo`);
    }

    function onCheck(currentValue: boolean) {
        zalgo.update((n) => {
            n.enabled = currentValue;
            return n;
        });
        if (!currentValue) {
            submitting.set(true);
            API.put(`/guilds/${$currentParams.guildId}/zalgo`, $zalgo)
                .then((res: IZalgoConfig) => {
                    zalgo.update(() => {
                        return res;
                    });
                    toastSuccess($_("guilds.zalgo.saved"));
                    clearCache();
                    submitting.set(false);
                })
                .catch(() => {
                    toastError($_("guilds.zalgo.failedtosave"));
                    submitting.set(false);
                });
        }
    }

    function onSubmit() {
        submitting.set(true);
        API.put(`/guilds/${$currentParams.guildId}/zalgo`, $zalgo)
            .then((res: IZalgoConfig) => {
                zalgo.update(() => {
                    return res;
                });
                toastSuccess($_("guilds.zalgo.saved"));
                clearCache();
                submitting.set(false);
            })
            .catch(() => {
                toastError($_("guilds.zalgo.failedtosave"));
                submitting.set(false);
            });
    }

    function onSimulate() {
        modalSubmitting.set(true);
        showModal.set(true);
        API.post(`/guilds/${$currentParams.guildId}/zalgo`, $zalgo, CacheMode.API_ONLY, false)
            .then((res: { oldName: string; newName: string; user: IDiscordUser }[]) => {
                simulateResult.update(() => {
                    return res;
                });
            })
            .finally(() => {
                modalSubmitting.set(false);
            });
    }

    function onModalClose() {
        showModal.set(false);
        setTimeout(() => {
            modalSubmitting.set(false);
            simulateResult.set([]);
        }, 400);
    }
</script>

<Modal
    size="sm"
    passiveModal
    open={$showModal}
    selectorPrimaryFocus="#messagetitleselection"
    modalHeading={$_("guilds.zalgo.simulateresults")}
    shouldSubmitOnEnter={false}
    on:close={onModalClose}>
    <Loading active={$modalSubmitting} />
    {#each $simulateResult as result}
        <div class="flex flex-row items-center mb-2 w-full">
            <UserIcon class="mr-2" user={result.user} />
            <div>{result.oldName}</div>
            <div class="mx-2">
                <ArrowRight20 />
            </div>
            <div>{result.newName}</div>
        </div>
    {/each}
</Modal>

<MediaQuery query="(min-width: 768px)" let:matches>
    <!-- Header -->
    <div class="flex flex-col {matches ? 'w-1/2' : ''}">
        <div class="text-3xl font-bold mb-4">
            {$_("guilds.zalgo.title")}
        </div>
        <div class="text-md">{$_("guilds.zalgo.explain")}</div>
    </div>

    <!-- Zalgo -->
    <div class="mt-4 {matches ? 'w-1/2' : ''}">
        {#if !$loading && !$zalgo}
            <div class="flex flex-col ml-2" class:items-center={!matches}>
                <Warning_02 />
                <div class="text-lg font-bold">{$_("guilds.zalgo.noresult")}</div>
            </div>
        {:else if $loading}
            <div>
                <Checkbox skeleton />
            </div>
            <div class="mb-2">
                <NumberInputSkeleton />
            </div>
            <div class="mb-2">
                <Checkbox skeleton />
            </div>
            <div class="mb-2">
                <TextInputSkeleton />
            </div>
            <div class="flex flex-row justify-end">
                <div>
                    <Button skeleton />
                </div>
            </div>
        {:else}
            <div class="mb-4">
                <Checkbox
                    labelText={$_("guilds.zalgo.enable")}
                    checked={$zalgo?.enabled}
                    disabled={$submitting}
                    on:check={(e) => {
                        onCheck(e.detail);
                    }} />
            </div>
            <div class="mb-2">
                <NumberInput
                    label={$_("guilds.zalgo.allowedpercentage")}
                    bind:value={$zalgo.percentage}
                    disabled={$submitting || !$zalgo.enabled}
                    min={0}
                    max={100} />
            </div>
            <div class="mb-2">
                <Checkbox
                    labelText={$_("guilds.zalgo.renamenormal")}
                    checked={$zalgo?.renameNormal}
                    disabled={$submitting || !$zalgo.enabled}
                    on:check={(e) => {
                        zalgo.update((n) => {
                            n.renameNormal = e.detail;
                            return n;
                        });
                    }} />
            </div>
            <div class="mb-2">
                <TextInput
                    labelText={$_("guilds.zalgo.renamefallback")}
                    placeholder={$_("guilds.zalgo.renamefallback")}
                    bind:value={$zalgo.renameFallback}
                    invalid={!$zalgo.renameFallback.trim() || $zalgo.renameFallback.trim().length > 32}
                    disabled={$submitting || !$zalgo.enabled} />
            </div>
            <div class="mb-2">
                <Checkbox
                    labelText={$_("guilds.zalgo.logtomodchannel")}
                    checked={$zalgo?.logToModChannel}
                    disabled={$submitting || !$zalgo.enabled}
                    on:check={(e) => {
                        zalgo.update((n) => {
                            n.logToModChannel = e.detail;
                            return n;
                        });
                    }} />
            </div>
            <div class="flex flex-row justify-end">
                <Button
                    kind="secondary"
                    on:click={onSimulate}
                    disabled={$submitting ||
                        !$zalgo?.enabled ||
                        ($zalgo?.percentage ?? 0) < 0 ||
                        ($zalgo?.percentage ?? 0) > 100 ||
                        !$zalgo.renameFallback.trim() ||
                        $zalgo.renameFallback.trim().length > 32}>
                    {$_("guilds.zalgo.simulate")}</Button>
                <div class="ml-2">
                    {#if $submitting}
                        <InlineLoading status="active" description={$_("guilds.zalgo.submitting")} />
                    {:else}
                        <Button
                            on:click={onSubmit}
                            disabled={$submitting ||
                                !$zalgo?.enabled ||
                                ($zalgo?.percentage ?? 0) < 0 ||
                                ($zalgo?.percentage ?? 0) > 100 ||
                                !$zalgo.renameFallback.trim() ||
                                $zalgo.renameFallback.trim().length > 32}>
                            {$_("guilds.zalgo.save")}</Button>
                    {/if}
                </div>
            </div>
        {/if}
    </div>
</MediaQuery>
