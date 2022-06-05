<script lang="ts">
    import type { ILabelUsage } from "./../../../models/api/ILabelUsage";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import { _ } from "svelte-i18n";
    import type { ITemplateView } from "../../../models/api/ITemplateView";
    import type { IDiscordUser } from "../../../models/discord/IDiscordUser";
    import { currentParams } from "../../../stores/currentParams";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import {
        Button,
        Checkbox,
        ComboBox,
        DatePicker,
        DatePickerInput,
        FileUploader,
        InlineLoading,
        Loading,
        Modal,
        Select,
        SelectItem,
        SelectSkeleton,
        Tag,
        TextArea,
        TextInput,
        TimePicker,
    } from "carbon-components-svelte";
    import Autocomplete from "../../../core/Autocomplete.svelte";
    import type { ICase } from "../../../models/api/ICase";
    import { writable } from "svelte/store";
    import type { Writable } from "svelte/store";
    import PunishmentTypes from "../../../services/enums/PunishmentType";
    import ViewPermissions from "../../../services/enums/ViewPermission";
    import { PunishmentType } from "../../../models/api/PunishmentType";
    import { currentFlatpickrLocale } from "../../../stores/currentLanguage";
    import { currentLanguage } from "../../../stores/currentLanguage";
    import moment from "moment";
    import type { ILanguageSelect } from "../../../models/ILanguageSelect";
    import { goto } from "@roxi/routify";
    import { toastError, toastSuccess, toastWarning } from "../../../services/toast/store";
    import { slide } from "svelte/transition";

    const utcOffset = new Date().getTimezoneOffset() * -1;

    let labels: { id: string; text: string }[] = [];

    let labelRef;
    let selectedLabels: Writable<string[]> = writable([]);

    let submitting: boolean = false;

    let inputPunishedUntilDate: any;
    let inputPunishedUntilTime: any;

    let modCase: ICase = {
        punishmentType: PunishmentType.None,
    } as ICase;

    let sendDmNotification: boolean = false;
    let executePunishment: boolean = true;
    let sendPublicNotification: boolean = false;

    $: $currentParams?.guildId ? loadGuildData() : null;
    function loadGuildData() {
        labels = [];
        API.get(`/guilds/${$currentParams.guildId}/cases/labels`, CacheMode.PREFER_CACHE, true).then((response: ILabelUsage[]) => {
            labels = response.map((x) => ({
                id: x.label,
                text: `${x.label} (${x.count})`,
            }));
        });
    }

    $: $currentParams?.caseId ? loadCaseData() : null;
    function loadCaseData() {
        API.get(`/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}`, CacheMode.PREFER_CACHE, true).then((response: ICase) => {
            modCase = response;
            modCase.punishmentType = response.punishmentType.toString();
            selectedLabels.set(response.labels);
            inputPunishedUntilDate = response.punishedUntil
                ? moment(response.punishedUntil)
                      .format($currentLanguage?.momentDateFormat ?? "YYYY-MM-DD")
                : "";
            inputPunishedUntilTime = response.punishedUntil
                ? moment(response.punishedUntil)
                      .format($currentLanguage?.momentTimeFormat ?? "HH:mm")
                : "";
        });
    }

    function onLabelSelect(e: { detail: { selectedId: string } }) {
        selectedLabels.update((n) => {
            n.push(e.detail.selectedId);
            return n;
        });
        labelRef.clear({ focus: false });
    }

    function onLabelRemove(label: string) {
        selectedLabels.update((n) => {
            n.splice(n.indexOf(label), 1);
            return n;
        });
    }

    $: calculatePunishedUntil(inputPunishedUntilDate, inputPunishedUntilTime, $currentLanguage);
    function calculatePunishedUntil(date: string, time: string, language?: ILanguageSelect) {
        if (language) {
            date
                ? (modCase.punishedUntil = moment(`${date} ${time ? time : "00:00"}`, `${language.momentDateFormat} ${language.momentTimeFormat}`)
                      .utc(false)
                      .utcOffset(utcOffset))
                : (modCase.punishedUntil = null);
        }
    }

    function onSubmit() {
        submitting = true;
        const body = {
            ...modCase,
            title: modCase.title,
            description: modCase.description ? modCase.description : modCase.title,
            userid: modCase.userId,
            labels: $selectedLabels,
            punishmentType: modCase.punishmentType,
            punishedUntil:
                modCase.punishmentType == PunishmentType.Mute || modCase.punishmentType == PunishmentType.Ban
                    ? modCase.punishedUntil?.toISOString()
                    : null,
        };

        API.put(
            `/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}?sendDmNotification=${sendDmNotification}&handlePunishment=${executePunishment}&sendPublicNotification=${sendPublicNotification}`,
            body
        )
            .then((res: { caseId: number }) => {
                submitting = false;
                toastSuccess($_("guilds.casedialog.caseedited", { values: { id: res.caseId } }));
                API.clearCacheEntryLike("get", `/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}`);
                API.clearCacheEntryLike("post", `/guilds/${$currentParams.guildId}/modcasetable`);
                $goto(`/guilds/${$currentParams.guildId}/cases/${res.caseId}`);
            })
            .catch((err) => {
                toastError($_("guilds.casedialog.caseeditfailed"));
                submitting = false;
            });
    }
</script>

<Loading active={submitting} />
<MediaQuery query="(min-width: 768px)" let:matches>
    <!-- Header -->
    <div class="flex flex-col {matches ? 'w-1/2' : ''}">
        <div class="text-3xl font-bold mb-4">
            {$_("guilds.casedialog.editcase")}
        </div>
        <div class="text-md">{$_("guilds.casedialog.caseexplain")}</div>
        <div class="text-md mb-4">{$_("guilds.casedialog.caseexplain2")}</div>

        <!-- Reason -->

        <div class="flex flex-col mb-4">
            <div class="text-lg font-bold mb-2">{$_("guilds.casedialog.reason")}</div>
            <div class="mb-2">
                <TextInput
                    bind:value={modCase.title}
                    labelText={$_("guilds.casedialog.title")}
                    placeholder={$_("guilds.casedialog.titleplaceholder")}
                    required
                    invalid={modCase.title !== undefined && modCase.title !== null && (modCase.title?.length ?? 0) > 100}
                    invalidText={$_("guilds.casedialog.titleinvalid")} />
            </div>
            <div class="mb-2">
                <TextArea
                    bind:value={modCase.description}
                    labelText={$_("guilds.casedialog.descriptionfield")}
                    placeholder={$_("guilds.casedialog.descriptionfield")} />
            </div>
            <div class="mb-2">
                {#if $selectedLabels}
                    <div class="flex flex-row flex-wrap mb-2" transition:slide|local>
                        {#each $selectedLabels as label (label)}
                            <Tag
                                type="outline"
                                filter
                                title={$_("guilds.casedialog.clearlabel")}
                                on:close={() => {
                                    onLabelRemove(label);
                                }}>{label}</Tag>
                        {/each}
                    </div>
                {/if}
                <Autocomplete
                    autoSelect={false}
                    bind:ref={labelRef}
                    items={labels}
                    placeholder={$_("guilds.casedialog.labelsplaceholder")}
                    on:select={onLabelSelect} />
            </div>
        </div>

        <!-- Punishment -->

        <div class="flex flex-col mb-4">
            <div class="text-lg font-bold mb-2">{$_("guilds.casedialog.punishment")}</div>
            <div class="text-md">{$_("guilds.casedialog.punishmentexplained")}</div>
            <div class="text-md mb-4">{$_("guilds.casedialog.punishmentexplained2")}</div>
            <Select bind:selected={modCase.punishmentType}>
                {#each PunishmentTypes.getAll() as type (type)}
                    <SelectItem value={type.id.toString()} text={$_(type.translationKey)} />
                {/each}
            </Select>

            <!-- Temporary punishment -->

            {#if modCase.punishmentType == PunishmentType.Mute || modCase.punishmentType == PunishmentType.Ban}
                <div class="flex flex-row mt-2" class:flex-wrap={!matches} transition:slide|local>
                    <DatePicker
                        class="!grow-0 !shrink mr-4"
                        bind:value={inputPunishedUntilDate}
                        datePickerType="single"
                        locale={$currentFlatpickrLocale ?? "en"}
                        dateFormat={$currentLanguage?.dateFormat ?? "m/d/Y"}
                        on:change>
                        <DatePickerInput labelText={$_("guilds.casedialog.punisheduntil")} placeholder={$currentLanguage?.dateFormat ?? "m/d/Y"} />
                    </DatePicker>
                    <TimePicker
                        class="!grow-0"
                        bind:value={inputPunishedUntilTime}
                        invalid={!!inputPunishedUntilTime && !/([01][012]|[1-9]):[0-5][0-9](\\s)?/.test(inputPunishedUntilTime)}
                        invalidText={$_("guilds.casedialog.formatisrequired", { values: { format: $currentLanguage?.timeFormat ?? "hh:MM" } })}
                        labelText={$_("guilds.casedialog.punisheduntil")}
                        placeholder={$currentLanguage?.timeFormat ?? "hh:MM"} />
                </div>
            {/if}
        </div>

        <!-- Submit -->

        <div class="mt-2">
            <Checkbox labelText={$_("guilds.casedialog.senddmnotification")} bind:checked={sendDmNotification} />
        </div>
        {#if modCase.punishmentType == PunishmentType.Mute || modCase.punishmentType == PunishmentType.Ban}
            <div transition:slide|local>
                <Checkbox labelText={$_("guilds.casedialog.executepunishment")} bind:checked={executePunishment} />
            </div>
        {/if}
        <div>
            <Checkbox labelText={$_("guilds.casedialog.sendpublicnotification")} bind:checked={sendPublicNotification} />
        </div>

        <div class="flex flex-row items-center justify-end mt-4">
            <div>
                {#if submitting}
                    <InlineLoading status="active" description={$_("guilds.casedialog.submitting")} />
                {:else}
                    <Button
                        on:click={onSubmit}
                        disabled={modCase.title === undefined ||
                            modCase.title === null ||
                            (modCase.title?.length ?? 0) === 0 ||
                            (modCase.title?.length ?? 0) > 100 ||
                            modCase.userId === undefined ||
                            modCase.userId === null ||
                            modCase.punishmentType === undefined ||
                            modCase.punishmentType === null}>{$_("guilds.casedialog.editcase")}</Button>
                {/if}
            </div>
        </div>
    </div>
</MediaQuery>
