<script lang="ts">
    import { Button, Checkbox, InlineLoading, SkeletonText, TextArea } from "carbon-components-svelte";
    import Warning_02 from "carbon-pictograms-svelte/lib/Warning_02.svelte";
    import { _ } from "svelte-i18n";
    import { writable, Writable } from "svelte/store";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import type { IMotd } from "../../../models/api/IMotd";
    import type { IMotdView } from "../../../models/api/IMotdView";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import { toastError, toastSuccess } from "../../../services/toast/store";
    import { currentParams } from "../../../stores/currentParams";

    const loading: Writable<boolean> = writable(false);
    const submitting: Writable<boolean> = writable(false);
    const motd: Writable<IMotdView> = writable(null);

    $: $currentParams?.guildId ? loadGuildData() : null;
    function loadGuildData() {
        loading.set(true);
        motd.set(null);
        submitting.set(false);

        API.get(`/guilds/${$currentParams.guildId}/motd`, CacheMode.PREFER_CACHE, true)
            .then((response: IMotdView) => {
                motd.set(response);
                loading.set(false);
            })
            .catch(() => {
                motd.set({
                    motd: {
                        guildId: $currentParams.guildId,
                        message: "",
                        showMotd: false,
                    }
                } as any)
                loading.set(false);
            });
    }

    function clearCache() {
        API.clearCacheEntry("get", `/guilds/${$currentParams.guildId}/motd`);
    }

    function onCheck(currentValue: boolean) {
        motd.update((n) => {
            n.motd.showMotd = currentValue;
            return n;
        });
        if (!currentValue) {
            submitting.set(true);
            API.put(`/guilds/${$currentParams.guildId}/motd`, $motd.motd)
                .then((res: IMotd) => {
                    motd.update((n) => {
                        n.motd = res;
                        return n;
                    });
                    toastSuccess($_("guilds.motd.saved"));
                    clearCache();
                    submitting.set(false);
                })
                .catch(() => {
                    toastError($_("guilds.motd.failedtosave"));
                    submitting.set(false);
                });
        }
    }

    function onSubmit() {
        submitting.set(true);
        API.put(`/guilds/${$currentParams.guildId}/motd`, $motd.motd)
            .then((res: IMotd) => {
                motd.update((n) => {
                    n.motd = res;
                    return n;
                });
                toastSuccess($_("guilds.motd.saved"));
                clearCache();
                submitting.set(false);
            })
            .catch(() => {
                toastError($_("guilds.motd.failedtosave"));
                submitting.set(false);
            });
    }
</script>

<MediaQuery query="(min-width: 768px)" let:matches>
    <!-- Header -->
    <div class="flex flex-col {matches ? 'w-1/2' : ''}">
        <div class="text-3xl font-bold mb-4">
            {$_("guilds.motd.title")}
        </div>
        <div class="text-md">{$_("guilds.motd.explain")}</div>
        <div class="text-md mb-4">{$_("guilds.motd.explain2")}</div>
    </div>

    <!-- Motd -->
    <div class="mt-4 {matches ? 'w-1/2' : ''}">
        {#if !$loading && !$motd}
            <div class="flex flex-col ml-2" class:items-center={!matches}>
                <Warning_02 />
                <div class="text-lg font-bold">{$_("guilds.motd.noresult")}</div>
            </div>
        {:else if $loading}
            <div>
                <Checkbox skeleton />
            </div>
            <div class="mb-2">
                <SkeletonText paragraph lines={matches ? 5 : 3} />
            </div>
            <div class="flex flex-row justify-end">
                <div>
                    <Button skeleton />
                </div>
            </div>
        {:else}
            <div>
                <Checkbox
                    labelText={$_("guilds.motd.enabled")}
                    checked={$motd?.motd?.showMotd}
                    disabled={$submitting}
                    on:check={(e) => {
                        onCheck(e.detail);
                    }} />
            </div>
            <div class="mb-2">
                <TextArea
                    value={$motd?.motd?.message}
                    rows={matches ? 10 : 5}
                    disabled={$submitting || !$motd?.motd?.showMotd}
                    placeholder={$_("guilds.motd.message")}
                    on:change={(e) => {
                        motd.update((n) => {
                            n.motd.message = e.target.value;
                            return n;
                        });
                    }} />
            </div>
            <div class="flex flex-row justify-end">
                <div>
                    {#if $submitting}
                        <InlineLoading status="active" description={$_("guilds.motd.submitting")} />
                    {:else}
                        <Button
                            on:click={onSubmit}
                            disabled={$submitting ||
                                !$motd?.motd?.showMotd ||
                                $motd?.motd?.message === null ||
                                ($motd?.motd?.message?.length ?? 0) === 0}>
                            {$_("guilds.motd.save")}</Button>
                    {/if}
                </div>
            </div>
        {/if}
    </div>
</MediaQuery>
