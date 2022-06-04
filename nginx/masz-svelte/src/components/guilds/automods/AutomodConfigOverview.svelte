<script lang="ts">
    import { slide, fade } from "svelte/transition";
    import type { IAutomodConfig } from "./../../../models/api/IAutomodConfig";
    import { Checkbox, Modal, MultiSelect, Select, SelectItem, TextArea, TextInput } from "carbon-components-svelte";
    import { types } from "./configOverview";
    import automodTypes from "../../../services/enums/AutomoderationType";
    import automodActions from "../../../services/enums/AutomoderationAction";
    import { _ } from "svelte-i18n";
    import { writable, Writable } from "svelte/store";
    import API from "../../../services/api/api";
    import { currentParams } from "../../../stores/currentParams";
    import { CacheMode } from "../../../services/api/CacheMode";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import AutomoderationActionTag from "../../api/AutomoderationActionTag.svelte";
    import { AutomodAction } from "../../../models/api/AutomodActionEnum";
    import PunishmentTag from "../../api/PunishmentTag.svelte";
    import { Edit32 } from "carbon-icons-svelte";
    import type { AutomodType } from "../../../models/api/AutomodType";
    import type { IAutoModRuleDefinition } from "../../../models/IAutoModRuleDefinition";
    import type { IDiscordChannel } from "../../../models/discord/IDiscordChannel";
    import PunishmentTypes from "../../../services/enums/PunishmentType";
    import ChannelNotificationBehavior from "../../../services/enums/AutomoderationChannelNotificationBehavior";
    import { PunishmentType } from "../../../models/api/PunishmentType";
    import { toastSuccess, toastError } from "../../../services/toast/store";

    const loading: Writable<boolean> = writable(false);
    const automods: Writable<IAutomodConfig[]> = writable([]);
    const submitting: Writable<boolean> = writable(false);
    const showEditModal: Writable<boolean> = writable(false);
    const editModalConfig: Writable<IAutomodConfig> = writable(null);
    const editModalRule: Writable<IAutoModRuleDefinition> = writable(null);
    const channels: Writable<{ id: string; text: string }[]> = writable([]);

    let roles;
    $: roles = ($currentParams?.guild?.roles ?? [])
        .filter(g => g.id != $currentParams.guildId)
        .sort((a, b) => (a.position < b.position ? 1 : -1))
        .map((role) => ({
            id: role.id,
            text: role.name,
        }));

    $: $currentParams?.guildId ? loadGuildData() : null;
    function loadGuildData() {
        loading.set(true);
        automods.set([]);

        API.get(`/guilds/${$currentParams.guildId}/automoderationconfig`, CacheMode.PREFER_CACHE, true)
            .then((response: IAutomodConfig[]) => {
                automods.set(response);
                setTimeout(() => {
                    loading.set(false);
                }, 200);
            })
            .catch(() => {
                loading.set(false);
            });

        API.get(`/discord/guilds/${$currentParams.guildId}/channels`, CacheMode.PREFER_CACHE, true).then((response) => {
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
    }

    function clearCache() {
        API.clearCacheEntry("get", `/guilds/${$currentParams.guildId}/automoderationconfig`);
    }

    function onCheck(rule: IAutoModRuleDefinition, checked: boolean) {
        if (!$loading) {
            if (checked) {
                onEdit(rule);
            } else {
                const prevConfig = $automods.find((x) => x.autoModerationType === rule.type);
                if (prevConfig) {
                    automods.update((n) => {
                        return n.filter((x) => x.autoModerationType !== rule.type);
                    });
                    API.deleteData(`/guilds/${$currentParams.guildId}/automoderationconfig/${rule.type}`, {})
                        .then(() => {
                            toastSuccess($_("guilds.automods.deactivated"));
                            clearCache();
                        })
                        .catch(() => {
                            automods.update((n) => {
                                n.push(prevConfig);
                                return n;
                            });
                            toastError($_("guilds.automods.failedtodeactivate"));
                        });
                }
            }
        }
    }

    function onEdit(rule: IAutoModRuleDefinition) {
        let config: IAutomodConfig = $automods.find((x) => x.autoModerationType === rule.type);
        if (!config) {
            config = {
                guildId: $currentParams.guildId,
                autoModerationAction: AutomodAction.None,
                autoModerationType: rule.type,
                ignoreChannels: [],
                ignoreRoles: [],
            } as any;
        }
        editModalConfig.set({
            ...config,
            punishmentType: config.punishmentType ? config.punishmentType.toString() : null,
            autoModerationAction: config.autoModerationAction ? config.autoModerationAction.toString() : null,
        } as any);
        editModalRule.set(rule);
        showEditModal.set(true);
    }

    function onModalClose() {
        showEditModal.set(false);
        setTimeout(() => {
            editModalConfig.set(null);
            editModalRule.set(null);
        }, 200);
    }

    function onModalConfirm() {
        submitting.set(true);
        API.put(`/guilds/${$currentParams.guildId}/automoderationconfig`, {
            ...$editModalConfig,
            autoModerationAction: $editModalConfig.autoModerationAction ?? AutomodAction.None,
        })
            .then((res) => {
                automods.update((n) => {
                    const index = n.findIndex((x) => x.autoModerationType === $editModalConfig.autoModerationType);
                    if (index === -1) {
                        n.push(res);
                    } else {
                        n[index] = res;
                    }
                    return n;
                });
                submitting.set(false);
                onModalClose();
                toastSuccess($_("guilds.automods.saved"));
                clearCache();
            })
            .catch(() => {
                submitting.set(false);
                toastError($_("guilds.automods.failedtosave"));
            });
    }
</script>

<style>
    .automod-widget {
        background-color: var(--cds-ui-01, #ffffff);
    }
</style>

<MediaQuery query="(min-width: 1280px)" let:matches>
    <!-- Header -->
    <div class="flex flex-col {matches ? 'w-1/2' : ''}">
        <div class="text-3xl font-bold mb-4">
            {$_("guilds.automods.title")}
        </div>
        <div class="text-md">{$_("guilds.automods.explain")}</div>
        <div class="text-md mb-4">{$_("guilds.automods.explain2")}</div>
    </div>

    <Modal
        hasForm
        shouldSubmitOnEnter={false}
        size="lg"
        open={$showEditModal}
        on:click:button--primary={onModalConfirm}
        on:click:button--secondary={onModalClose}
        on:close={onModalClose}
        primaryButtonDisabled={$submitting ||
            (!$editModalConfig?.limit && $editModalRule?.showLimitField) ||
            (!$editModalConfig?.timeLimitMinutes && $editModalRule?.showTimeLimitField) ||
            (!$editModalConfig?.customWordFilter && $editModalRule?.requireCustomField)}
        modalHeading={$_("guilds.automods.title") + " - " + $_(automodTypes.getById($editModalRule?.type))}
        primaryButtonText={$_("dialog.confirm.confirm")}
        secondaryButtonText={$_("dialog.confirm.cancel")}>
        <div class="min-h-screen">
            {#if $editModalRule && $editModalConfig}
                <div class="text-xl font-bold">{$_("guilds.automods.config")}</div>
                <div style="word-wrap: break-word;">
                    {$_(`guilds.automods.${$editModalRule?.key}.details`)}
                </div>
                <div class="grid grid-cols-2 gap-4 w-full mt-2">
                    {#if $editModalRule.showLimitField}
                        <TextInput
                            required={true}
                            readonly={$submitting}
                            labelText={$_(`guilds.automods.limitfield`)}
                            placeholder={$_(`guilds.automods.limitfield`)}
                            bind:value={$editModalConfig.limit} />
                    {/if}
                    {#if $editModalRule.showTimeLimitField}
                        <TextInput
                            required={true}
                            readonly={$submitting}
                            labelText={$_(`guilds.automods.timelimitfield`)}
                            placeholder={$_(
                                $editModalRule.timeLimitFieldMessage
                                    ? `guilds.automods.${$editModalRule?.key}.timelimitmessage`
                                    : `guilds.automods.timelimitfield`
                            )}
                            bind:value={$editModalConfig.timeLimitMinutes} />
                    {/if}
                </div>
                {#if $editModalRule.showCustomField}
                    <div class="mt-2">
                        <TextArea
                            readonly={$submitting}
                            required={$editModalRule.requireCustomField}
                            labelText={$_(`guilds.automods.${$editModalRule?.key}.customwordfield`)}
                            placeholder={$_(`guilds.automods.${$editModalRule?.key}.customwordfield`)}
                            bind:value={$editModalConfig.customWordFilter} />
                    </div>
                {/if}

                <div class="text-xl font-bold mt-4">{$_("guilds.automods.filter")}</div>

                <div class="grid grid-cols-1 md:grid-cols-2 md:gap-4 w-full">
                    <div>
                        <MultiSelect
                            readonly={$submitting}
                            titleText={$_("guilds.automods.filterroles")}
                            items={roles}
                            selectedIds={$editModalConfig.ignoreRoles ?? []}
                            on:select={(e) => {
                                $editModalConfig.ignoreRoles = e.detail.selectedIds;
                            }} />
                    </div>
                    <div>
                        <MultiSelect
                            readonly={$submitting}
                            titleText={$_("guilds.automods.filterchannels")}
                            items={$channels}
                            selectedIds={$editModalConfig?.ignoreChannels ?? []}
                            on:select={(e) => {
                                $editModalConfig.ignoreChannels = e.detail.selectedIds;
                            }} />
                    </div>
                </div>

                <div class="text-xl font-bold mt-4">{$_("guilds.automods.action")}</div>

                <div>
                    <Checkbox
                        labelText={$_("guilds.casedialog.senddmnotification")}
                        bind:checked={$editModalConfig.sendDmNotification}
                        readonly={$submitting} />
                </div>
                {#if $editModalConfig.autoModerationAction == AutomodAction.CaseCreated || $editModalConfig.autoModerationAction == AutomodAction.ContentDeletedAndCaseCreated}
                    <div transition:slide|local>
                        <Checkbox
                            labelText={$_("guilds.casedialog.sendpublicnotification")}
                            bind:checked={$editModalConfig.sendPublicNotification}
                            readonly={$submitting} />
                    </div>
                {/if}

                <div class="grid grid-cols-1 md:grid-cols-2 gap-4 items-center w-full mt-2">
                    <div>
                        <Select bind:selected={$editModalConfig.autoModerationAction} hideLabel readonly={$submitting}>
                            {#each automodActions.getAll() as type (type)}
                                <SelectItem value={type.id.toString()} text={$_(type.translationKey)} />
                            {/each}
                        </Select>
                    </div>
                    {#if $editModalConfig.autoModerationAction == AutomodAction.CaseCreated || $editModalConfig.autoModerationAction == AutomodAction.ContentDeletedAndCaseCreated}
                        <div transition:fade|local>
                            <Select bind:selected={$editModalConfig.punishmentType} hideLabel readonly={$submitting}>
                                {#each PunishmentTypes.getAll() as type (type)}
                                    <SelectItem value={type.id.toString()} text={$_(type.translationKey)} />
                                {/each}
                            </Select>
                        </div>
                    {:else}
                        <div />
                    {/if}
                    {#if $editModalConfig.autoModerationAction == AutomodAction.ContentDeleted || $editModalConfig.autoModerationAction == AutomodAction.ContentDeletedAndCaseCreated}
                        <div transition:fade|local>
                            <Select bind:selected={$editModalConfig.channelNotificationBehavior} hideLabel readonly={$submitting}>
                                {#each ChannelNotificationBehavior.getAll() as type (type)}
                                    <SelectItem value={type.id.toString()} text={$_(type.translationKey)} />
                                {/each}
                            </Select>
                        </div>
                    {/if}
                    {#if $editModalConfig.autoModerationAction == AutomodAction.Timeout || (($editModalConfig.punishmentType == PunishmentType.Mute || $editModalConfig.punishmentType == PunishmentType.Ban) && ($editModalConfig.autoModerationAction == AutomodAction.CaseCreated || $editModalConfig.autoModerationAction == AutomodAction.ContentDeletedAndCaseCreated))}
                        <div transition:fade|local>
                            <TextInput
                                readonly={$submitting}
                                placeholder={$_(`guilds.automods.punishmentduration`)}
                                bind:value={$editModalConfig.punishmentDurationMinutes} />
                        </div>
                    {/if}
                </div>
            {/if}
        </div>
    </Modal>

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 w-full" style={matches ? "grid-auto-rows: 1fr;" : ""}>
        {#each Array.from(types) as rule}
            <div class="p-4 automod-widget">
                <div class="flex flex-row items-center">
                    <div class="self-start">
                        <Checkbox
                            checked={$automods.find((x) => x.autoModerationType === rule.type) !== undefined}
                            on:check={(e) => {
                                onCheck(rule, e.detail);
                            }} />
                    </div>
                    <div class="font-bold text-md lg:text-xl">{$_(automodTypes.getById(rule.type))}</div>
                    <div class="grow" />
                    {#if $automods.find((x) => x.autoModerationType === rule.type) !== undefined}
                        <div class="cursor-pointer h-6 w-6 ml-2" transition:fade|local>
                            <Edit32
                                on:click={() => {
                                    onEdit(rule);
                                }} />
                        </div>
                    {/if}
                </div>
                {#if $automods.find((x) => x.autoModerationType === rule.type)}
                    <div transition:slide|local>
                        {#each [$automods.find((x) => x.autoModerationType === rule.type)] as automod (automod.autoModerationType)}
                            <div class="flex flex-row">
                                {#if automod.autoModerationAction !== AutomodAction.None}
                                    <AutomoderationActionTag action={automod.autoModerationAction} />
                                {/if}
                                {#if automod.autoModerationAction === AutomodAction.CaseCreated || automod.autoModerationAction === AutomodAction.ContentDeletedAndCaseCreated}
                                    <PunishmentTag modCase={{ punishmentActive: true, punishmentType: automod.punishmentType }} />
                                {/if}
                            </div>
                            <div class="mt-1" style="word-wrap: break-word;">
                                {$_(`guilds.automods.${rule.key}.details`)}
                            </div>
                        {/each}
                    </div>
                {/if}
            </div>
        {/each}
    </div>
</MediaQuery>
