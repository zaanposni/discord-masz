<script lang="ts">
    import { auditlogs } from "./auditlogs";
    import TranslatedAuditlogs from "../../../services/enums/GuildAuditlogEvent";
    import { _ } from "svelte-i18n";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import { Checkbox, ComboBox, MultiSelect, Select, SelectItem, SelectSkeleton } from "carbon-components-svelte";
    import type { Writable } from "svelte/store";
    import { writable } from "svelte/store";
    import { currentParams } from "../../../stores/currentParams";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import type { IDiscordChannel } from "../../../models/discord/IDiscordChannel";
    import type { IGuildAuditlogConfig } from "../../../models/api/IGuildAuditlogConfig";
    import { toastError, toastSuccess } from "../../../services/toast/store";

    const loading: Writable<boolean> = writable(true);
    const submitting: Writable<boolean> = writable(true);
    const data: Writable<Map<number, IGuildAuditlogConfig>> = writable(new Map());
    const channels: Writable<{ id: string; text: string }[]> = writable([]);

    let roles;
    $: roles = ($currentParams?.guild?.roles ?? [])
        .filter((g) => g.id != $currentParams.guildId)
        .sort((a, b) => (a.position < b.position ? 1 : -1))
        .map((role) => ({
            id: role.id,
            text: role.name,
        }));

    $: $currentParams?.guildId ? loadGuildData() : null;
    function loadGuildData() {
        loading.set(true);
        submitting.set(true);
        data.set(new Map());
        channels.set([]);

        const auditlogPromise = API.get(`/guilds/${$currentParams.guildId}/auditlog`, CacheMode.PREFER_CACHE, true).then(
            (response: IGuildAuditlogConfig[]) => {
                data.set(new Map(response.map((auditlog) => [auditlog.guildAuditLogEvent, auditlog])));
            }
        );

        const channelPromise = API.get(`/discord/guilds/${$currentParams.guildId}/channels`, CacheMode.PREFER_CACHE, true).then((response) => {
            channels.set(
                response
                    .filter((x) => x.type === 0)
                    .sort((a, b) => (a.position > b.position ? 1 : -1))
                    .map((channel: IDiscordChannel) => ({
                        id: channel.id,
                        text: channel.name,
                    }))
            );
        });

        Promise.all([auditlogPromise, channelPromise])
            .then(() => {
                loading.set(false);
                setTimeout(() => {
                    submitting.set(false);
                }, 300);
            })
            .catch(() => {
                loading.set(false);
                setTimeout(() => {
                    submitting.set(false);
                }, 300);
            });
    }

    function clearCache() {
        API.clearCacheEntry("get", `/guilds/${$currentParams.guildId}/auditlog`);
    }

    function dummyObject(type: number): IGuildAuditlogConfig {
        return {
            id: null,
            guildId: $currentParams.guildId,
            guildAuditLogEvent: type,
            channelId: null,
            pingRoles: [],
            ignoreChannels: [],
            ignoreRoles: [],
        };
    }

    function handleCheck(type: number, checked: boolean) {
        if (!$submitting) {
            if (checked) {
                submitting.set(true);
                data.update((n) => {
                    n.set(type, dummyObject(type));
                    return n;
                });
                setTimeout(() => {
                    submitting.set(false);
                }, 300);
            } else if ($data.get(type) !== undefined) {
                if (JSON.stringify($data.get(type)) === JSON.stringify(dummyObject(type))) {
                    data.update((n) => {
                        n.delete(type);
                        return n;
                    });
                } else {
                    deleteConfig(type);
                }
            }
        }
    }

    function deleteConfig(type: number) {
        if (!$submitting) {
            API.deleteData(`/guilds/${$currentParams.guildId}/auditlog/${type}`, {})
                .then(() => {
                    data.update((n) => {
                        n.delete(type);
                        return n;
                    });
                    toastSuccess($_("guilds.auditlog.saved"));
                    clearCache();
                })
                .catch(() => {
                    toastError($_("guilds.auditlog.failedtosave"));
                });
        }
    }

    function saveConfig(type: number) {
        if (!$submitting) {
            API.put(`/guilds/${$currentParams.guildId}/auditlog`, $data.get(type))
                .then(() => {
                    toastSuccess($_("guilds.auditlog.saved"));
                    clearCache();
                })
                .catch(() => {
                    toastError($_("guilds.auditlog.failedtosave"));
                });
        }
    }

    function saveProp(type: number, prop: string, value: any) {
        if (!$submitting) {
            data.update((n) => {
                n.set(type, {
                    ...n.get(type),
                    [prop]: value,
                });
                return n;
            });
            saveConfig(type);
        }
    }
</script>

<MediaQuery query="(min-width: 1280px)" let:matches>
    <!-- Header -->
    <div class="flex flex-col {matches ? 'w-1/2' : ''}">
        <div class="text-3xl font-bold mb-4">
            {$_("guilds.auditlog.title")}
        </div>
        <div class="text-md">{$_("guilds.auditlog.explain")}</div>
        <div class="text-md mb-4">{$_("guilds.auditlog.explain2")}</div>
    </div>

    <div class="grid grid-cols-5 gap-2 items-center mt-4 {matches ? 'w-4/5' : ''}" style={matches ? "grid-auto-rows: 1fr;" : ""}>
        {#if $loading}
            {#each new Array(20) as _}
                <div class="col-span-5" class:!col-span-1={matches}>
                    <Checkbox skeleton />
                </div>
                <div class="grid grid-cols-4 gap-2 items-center col-span-5" class:!col-span-4={matches} style={matches ? "grid-auto-rows: 1fr;" : ""}>
                    <div class="grid grid-cols-4 gap-2 items-center col-span-4">
                        <div class:!col-span-2={!matches}>
                            <SelectSkeleton hideLabel />
                        </div>
                        <div class:!col-span-2={!matches}>
                            <SelectSkeleton hideLabel />
                        </div>
                        <div class:!col-span-2={!matches}>
                            <SelectSkeleton hideLabel />
                        </div>
                        <div class:!col-span-2={!matches}>
                            <SelectSkeleton hideLabel />
                        </div>
                    </div>
                </div>
            {/each}
        {:else}
            {#each auditlogs as auditlog (auditlog.type)}
                <div class="col-span-5" class:!col-span-1={matches}>
                    <Checkbox
                        labelText={$_(TranslatedAuditlogs.getById(auditlog.type))}
                        checked={$data.get(auditlog.type) !== undefined}
                        on:check={(e) => {
                            handleCheck(auditlog.type, e.detail);
                        }} />
                </div>
                <div class="grid grid-cols-4 gap-2 items-center col-span-5" class:!col-span-4={matches} style={matches ? "grid-auto-rows: 1fr;" : ""}>
                    {#if $data.get(auditlog.type) !== undefined}
                        <div class="grid grid-cols-4 gap-2 items-center col-span-4">
                            <div class:!col-span-2={!matches}>
                                <ComboBox
                                    shouldFilterItem={(item, value) => {
                                        return `#${item.text.toLowerCase()}`.includes(value.toLowerCase());
                                    }}
                                    titleText={$_("guilds.auditlog.targetchannel")}
                                    items={$channels}
                                    selectedId={$data.get(auditlog.type)?.channelId ?? undefined}
                                    on:select={(e) => {
                                        saveProp(auditlog.type, "channelId", e.detail.selectedId);
                                    }} />
                            </div>
                            <div class:!col-span-2={!matches}>
                                <MultiSelect
                                    titleText={$_("guilds.auditlog.pingrole")}
                                    items={roles}
                                    selectedIds={$data.get(auditlog.type)?.pingRoles ?? []}
                                    on:select={(e) => {
                                        saveProp(auditlog.type, "pingRoles", e.detail.selectedIds);
                                    }} />
                            </div>
                            <div class:!col-span-2={!matches}>
                                {#if auditlog.roleFilter}
                                    <MultiSelect
                                        titleText={$_("guilds.auditlog.excluderoles")}
                                        items={roles}
                                        selectedIds={$data.get(auditlog.type)?.ignoreRoles ?? []}
                                        on:select={(e) => {
                                            saveProp(auditlog.type, "ignoreRoles", e.detail.selectedIds);
                                        }} />
                                {/if}
                            </div>
                            <div class:!col-span-2={!matches}>
                                {#if auditlog.channelFilter}
                                    <MultiSelect
                                        titleText={$_("guilds.auditlog.excludechannels")}
                                        items={$channels}
                                        selectedIds={$data.get(auditlog.type)?.ignoreChannels ?? []}
                                        on:select={(e) => {
                                            saveProp(auditlog.type, "ignoreChannels", e.detail.selectedIds);
                                        }} />
                                {/if}
                            </div>
                        </div>
                    {:else}
                        <div class="col-span-6" class:!col-span-4={matches} />
                    {/if}
                </div>
            {/each}
        {/if}
    </div>
</MediaQuery>
