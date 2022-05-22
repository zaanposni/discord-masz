<script lang="ts">
    import { _ } from "svelte-i18n";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import {
        Button,
        Checkbox,
        ComboBox,
        InlineLoading,
        MultiSelect,
        NumberInput,
        Select,
        SelectItem,
        SelectSkeleton,
        TextInput,
        Tooltip,
    } from "carbon-components-svelte";
    import { derived, Writable } from "svelte/store";
    import { writable } from "svelte/store";
    import { currentParams } from "../../../stores/currentParams";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import type { IGuildAuditlogConfig } from "../../../models/api/IGuildAuditlogConfig";
    import { toastError, toastSuccess } from "../../../services/toast/store";
    import type { IGuildConfig } from "../../../models/api/IGuildConfig";
    import Languages from "../../../services/enums/Language";
    import Language from "../../../services/enums/Language";

    const loading: Writable<boolean> = writable(true);
    const submitting: Writable<boolean> = writable(true);
    const data: Writable<IGuildConfig> = writable(null);
    const webhookRegex = new RegExp("^https://discord(app)?.com/api/webhooks/.*$");

    const publicWebhookInvalid = derived(data, ($data) => {
        if (!$data) return true;
        return $data.modPublicNotificationWebhook && !webhookRegex.test($data.modPublicNotificationWebhook);
    });
    const internalWebhookInvalid = derived(data, ($data) => {
        if (!$data) return true;
        return $data.modInternalNotificationWebhook && !webhookRegex.test($data.modInternalNotificationWebhook);
    });

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
        submitting.set(false);
        data.set(null);

        API.get(`/guilds/${$currentParams.guildId}`, CacheMode.PREFER_CACHE, true).then((response: IGuildConfig) => {
            data.set({
                ...response,
                preferredLanguage: response.preferredLanguage.toString(),
            });
            loading.set(false);
        });
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
</script>

<MediaQuery query="(min-width: 1024px)" let:matches>
    <!-- Header -->
    <div class="flex flex-col {matches ? 'w-1/2' : ''}">
        <div class="text-3xl font-bold mb-4">
            {$_("guilds.config.title")}
        </div>
    </div>

    {#if $loading}
        skeleton
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
