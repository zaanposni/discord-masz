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
        Tile,
    } from "carbon-components-svelte";
    import { Box24, CopyLink24, Unlink24 } from "carbon-icons-svelte";
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
    import { toastError, toastSuccess } from "../../../services/toast/store";
    import { slide } from "svelte/transition";
    import type { IVerifiedEvidenceCompactView } from "../../../models/api/IVerifiedEvidenceCompactView";
    import UserIcon from "../../discord/UserIcon.svelte";

    const utfOffset = new Date().getTimezoneOffset() * -1;

    let templatesLoading: boolean = true;
    let templates: { id: string; text: string; obj: ITemplateView }[] = [];
    let membersLoading: boolean = true;
    let members: { id: string; text: string }[] = [];
    let labels: { id: string; text: string }[] = [];

    let templateRef;
    let templateModalOpen: Writable<boolean> = writable(false);
    let templateModalTitle: string = "";
    let templateModalVisibility: string = "-1";
    let templateSubmitting: boolean = false;

    let evidenceModalOpen = writable(false);
    let evidenceModalSearch = writable("");
    let evidenceModalSearchResults = writable<IVerifiedEvidenceCompactView[]>([]);
    let evidenceModalSearching = writable(false);
    let linkedEvidence = writable<IVerifiedEvidenceCompactView[]>([]);

    let searchEvidenceDebouncer;
    function searchEvidence(search: string) {
        if (searchEvidenceDebouncer) {
            clearTimeout(searchEvidenceDebouncer);
        }

        if (search) {
            evidenceModalSearching.set(true);
            evidenceModalSearchResults.set([]);

            const data = {
                customTextFilter: search,
            };

            searchEvidenceDebouncer = setTimeout(() => {
                API.post(`/guilds/${$currentParams.guildId}/evidence/evidencetable?startPage=0`, data, CacheMode.API_ONLY, false)
                    .then((response: { evidence: IVerifiedEvidenceCompactView[]; fullSize: number }) => {
                        evidenceModalSearchResults.set(
                            response.evidence.filter(
                                (e) => !$linkedEvidence.some((x) => x.verifiedEvidence.id == e.verifiedEvidence.id)
                            )
                        );
                        evidenceModalSearching.set(false);
                    })
                    .catch(() => {
                        evidenceModalSearching.set(false);
                    });
            }, 500);
        }
    }
    $: searchEvidence($evidenceModalSearch);

    function onLinkEvidenceModalClosed() {
        evidenceModalOpen.set(false);
        setTimeout(() => {
            evidenceModalSearch.set("");
            evidenceModalSearching.set(false);
            evidenceModalSearchResults.set([]);
        }, 500);
    }

    function linkEvidence(evidence: IVerifiedEvidenceCompactView) {
        linkedEvidence.update(x => [evidence, ...x]);
        onLinkEvidenceModalClosed();
    }

    function unlinkEvidence(evidence: IVerifiedEvidenceCompactView) {
        linkedEvidence.set($linkedEvidence.filter(e => e.verifiedEvidence.id !== evidence.verifiedEvidence.id));
    }

    let labelRef;
    let selectedLabels: Writable<string[]> = writable([]);

    let submitting: boolean = false;

    let inputPunishedUntilDate: any;
    let inputPunishedUntilTime: any;

    const filesToUpload: Writable<File[]> = writable([]);

    let modCase: ICase = {
        punishmentType: PunishmentType.None,
    } as ICase;

    let sendDmNotification: boolean = true;
    let executePunishment: boolean = true;
    let sendPublicNotification: boolean = true;

    $: $currentParams?.guildId ? loadGuildData() : null;
    function loadGuildData() {
        templatesLoading = true;
        membersLoading = true;
        members = [];
        templates = [];
        labels = [];
        modCase = {
            punishmentType: PunishmentType.None,
        } as ICase;
        API.get(`/discord/guilds/${$currentParams.guildId}/members`, CacheMode.PREFER_CACHE, true)
            .then((response: IDiscordUser[]) => {
                members = response.map((x) => ({
                    id: x.id,
                    text: `${x.username}#${x.discriminator}`,
                }));
                membersLoading = false;
            })
            .catch(() => {
                membersLoading = false;
            });
        API.get(`/templatesview`, CacheMode.PREFER_CACHE, true)
            .then((response: ITemplateView[]) => {
                templates = response.map((x) => ({
                    id: x.caseTemplate.id.toString(),
                    text: `"${x.caseTemplate.templateName}" by ${x.creator.username}#${x.creator.discriminator} from ${
                        x.guild?.name ?? "Unknown guild"
                    }`,
                    obj: x,
                }));
                templatesLoading = false;
            })
            .catch(() => {
                templatesLoading = false;
            });
        API.get(`/guilds/${$currentParams.guildId}/cases/labels`, CacheMode.PREFER_CACHE, true).then((response: ILabelUsage[]) => {
            labels = response.map((x) => ({
                id: x.label,
                text: `${x.label} (${x.count})`,
            }));
        });
    }

    function shouldFilterTemplate(item: { id: string; text: string; obj: ITemplateView }, value: string) {
        if (!value) return true;
        value = value.toLowerCase();
        return (
            item.text.toLowerCase().includes(value) ||
            item.obj.caseTemplate.caseTitle.toLowerCase().includes(value) ||
            item.obj.caseTemplate.caseDescription.toLowerCase().includes(value) ||
            item.obj.caseTemplate.caseLabels.join("").toLocaleLowerCase().includes(value)
        );
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
                      .utcOffset(utfOffset))
                : (modCase.punishedUntil = null);
        }
    }

    function onSubmit() {
        submitting = true;
        const body = {
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

        API.post(
            `/guilds/${$currentParams.guildId}/cases?sendDmNotification=${sendDmNotification}&handlePunishment=${executePunishment}&sendPublicNotification=${sendPublicNotification}`,
            body,
            CacheMode.API_ONLY,
            false
        )
            .then((res: { caseId: number; id: number }) => {
                API.clearCacheEntryLike("post", `/guilds/${$currentParams.guildId}/modcasetable`);
                submitting = false;
                toastSuccess($_("guilds.casedialog.casecreated", { values: { id: res.caseId } }));
                $goto(`/guilds/${$currentParams.guildId}/cases/${res.caseId}`);

                $linkedEvidence.forEach((evidence) => {
                    linkEvidenceInApi(evidence, res.id);
                })

                $filesToUpload.forEach((file) => {
                    uploadFile(file, res.caseId);
                });
            })
            .catch((err) => {
                toastError($_("guilds.casedialog.casecreatefailed"));
                submitting = false;
            });
    }

    function linkEvidenceInApi(evidence: IVerifiedEvidenceCompactView, caseId: number) {
        API.post(`guilds/${$currentParams.guildId}/evidencemapping/${evidence.verifiedEvidence.id}/${caseId}`, null, CacheMode.API_ONLY, false)
            .catch(() => {
                toastError($_("guilds.caseview.evidencelinkfailed"))
            });
    }

    function uploadFile(file: File, caseId: number) {
        const formData: FormData = new FormData();
        formData.append("File", file, file.name);

        API.post(`/guilds/${$currentParams.guildId}/cases/${caseId}/files`, formData, CacheMode.API_ONLY, false).catch((err) => {
            toastSuccess($_("guilds.caseview.attachmentuploadfailed"));
        });
    }

    function applyTemplate(e: { detail: { selectedItem: { obj: ITemplateView } } }) {
        modCase.title = e.detail.selectedItem.obj.caseTemplate.caseTitle;
        modCase.description = e.detail.selectedItem.obj.caseTemplate.caseDescription;
        selectedLabels.update((n) => {
            n = e.detail.selectedItem.obj.caseTemplate.caseLabels;
            return n;
        });
        modCase.punishmentType = e.detail.selectedItem.obj.caseTemplate.casePunishmentType.toString();
        modCase.punishedUntil = e.detail.selectedItem.obj.caseTemplate.casePunishedUntil;

        sendDmNotification = e.detail.selectedItem.obj.caseTemplate.announceDm;
        executePunishment = e.detail.selectedItem.obj.caseTemplate.handlePunishment;
        sendPublicNotification = e.detail.selectedItem.obj.caseTemplate.sendPublicNotification;

        templateRef.clear({ focus: false });

        toastSuccess($_("guilds.casedialog.templateapplied"));
    }

    function onTemplateModalOpen() {
        templateModalOpen.set(true);
        templateModalTitle = "";
        templateModalVisibility = "-1";
        templateSubmitting = false;
    }

    function onTemplateModalClose() {
        templateModalOpen.set(false);
        templateModalTitle = "";
        templateModalVisibility = "-1";
        templateSubmitting = false;
    }

    function saveAsTemplate() {
        templateSubmitting = true;

        let body = {
            templateName: templateModalTitle,
            viewPermission: templateModalVisibility,
            title: modCase.title,
            description: modCase.description ? modCase.description : modCase.title,
            labels: $selectedLabels ?? [],
            punishmentType: modCase.punishmentType,
            punishedUntil:
                modCase.punishmentType == PunishmentType.Mute || modCase.punishmentType == PunishmentType.Ban
                    ? modCase.punishedUntil?.toISOString()
                    : null,
            announceDm: sendDmNotification,
            handlePunishment: executePunishment,
            sendPublicNotification: sendPublicNotification,
        };

        API.post(`/templates?guildId=${$currentParams.guildId}`, body, CacheMode.API_ONLY, false)
            .then(() => {
                toastSuccess($_("guilds.casedialog.templatesaved"));
                templateSubmitting = false;
                onTemplateModalClose();
            })
            .catch((err) => {
                toastError($_("guilds.casedialog.templatesavefailed"));
                templateSubmitting = false;
            });
    }

    document.addEventListener("paste", (e: ClipboardEvent) => {
        if (e.clipboardData) {
            const items = e.clipboardData.items;
            if (items) {
                for (let i = 0; i < items.length; i++) {
                    if (items[i].type.indexOf("image") !== -1) {
                        e.preventDefault();
                        const blob = items[i].getAsFile();
                        if (blob) {
                            filesToUpload.update(n => {
                                n.push(blob);
                                return n;
                            });
                        }
                    }
                }
            }
        }
    });

    function onLinkEvidenceButton() {
        evidenceModalOpen.set(true);
    }
</script>

<style>
    #evidence-explained {
        margin-bottom: 1rem;
    }
</style>

<Modal
    size="sm"
    open={$templateModalOpen}
    selectorPrimaryFocus="#templatename"
    modalHeading={$_("guilds.casedialog.createtemplate")}
    primaryButtonText={$_("guilds.casedialog.createtemplate")}
    secondaryButtonText={$_("dialog.confirm.cancel")}
    primaryButtonDisabled={templateSubmitting || (templateModalTitle?.length ?? 0) < 1 || !templateModalVisibility}
    on:close={onTemplateModalClose}
    on:click:button--secondary={onTemplateModalClose}
    on:submit={saveAsTemplate}>
    <Loading active={templateSubmitting} />
    <div class="mb-4">
        <TextInput
            id="templatename"
            bind:value={templateModalTitle}
            labelText={$_("guilds.casedialog.title")}
            placeholder={$_("guilds.casedialog.titleplaceholder")}
            required
            invalid={templateModalTitle !== undefined && templateModalTitle !== null && (templateModalTitle?.length ?? 0) > 100}
            invalidText={$_("guilds.casedialog.titleinvalid")} />
    </div>
    <div>
        <ComboBox
            items={ViewPermissions.getAll().map((x) => ({
                id: x.id.toString(),
                text: $_(x.translationKey),
            }))}
            direction="top"
            bind:selectedId={templateModalVisibility}
            titleText={$_("guilds.casedialog.visibility")}
            placeholder={$_("guilds.casedialog.visibility")} />
    </div>
</Modal>

<!-- Link evidence modal -->
<Modal
    size="sm"
    open={$evidenceModalOpen}
    selectorPrimaryFocus="#evidence-search"
    modalHeading={$_("guilds.caseview.linkevidence")}
    passiveModal
    on:close={onLinkEvidenceModalClosed}
>
    <div class="mb-2">
        <TextInput
            id="evidence-search"
            labelText={$_("guilds.caseview.search")}
            placeholder={$_("guilds.caseview.search")}
            bind:value={$evidenceModalSearch} 
        />
    </div>
    {#if $evidenceModalSearching}
        <div>
            <InlineLoading />
        </div>
    {:else}
        <div class="flex flex-col" transition:slide|local>
            {#each $evidenceModalSearchResults as evidence}
                <div transition:slide|local>
                    <Tile class="mb-2" light>
                        <div class="flex flex-row grow-0 w-full max-w-full items-center">
                            <Box24 class="shrink-0 mr-2" />
                            <div class="shrink-0 mr-2" style="color: var(--cds-text-02)">
                                #{evidence.verifiedEvidence.id}
                            </div>
                            <div class="shrink-0 mr-2">
                                <div class="flex flex-row flex-wrap items-center">
                                    <UserIcon class="self-start mr-2" user={evidence.reported}/>
                                    <div class="mr-2">
                                        {evidence.reported?.username ?? evidence.verifiedEvidence.username}#{evidence.reported?.discriminator ??
                                            evidence.verifiedEvidence.discriminator}
                                    </div>
                                </div>
                            </div>
                            <div class="grow truncate">
                                {evidence.verifiedEvidence.reportedContent.slice(0, 31)}
                            </div>
                            <div class="cursor-pointer">
                                <CopyLink24 class="mr-2" on:click={() => linkEvidence(evidence)} />
                            </div>
                        </div>
                    </Tile>
                </div>
            {:else}
                {#if $evidenceModalSearch}
                    {$_("guilds.caseview.noevidencefound")}
                {/if}
            {/each}
        </div>
    {/if}
</Modal>

<Loading active={submitting} />
<MediaQuery query="(min-width: 768px)" let:matches>
    <!-- Header -->
    <div class="flex flex-col {matches ? 'w-1/2' : ''}">
        <div class="text-3xl font-bold mb-4">
            {$_("guilds.casedialog.createcase")}
        </div>
        <div class="text-md">{$_("guilds.casedialog.caseexplain")}</div>
        <div class="text-md mb-4">{$_("guilds.casedialog.caseexplain2")}</div>

        <!-- Template -->

        <div class="flex flex-col mb-4">
            <div class="text-lg font-bold mb-2">{$_("guilds.casedialog.importtemplate")}</div>
            <div class="text-md mb-2">{$_("guilds.casedialog.templateexplain")}</div>
            {#if templatesLoading}
                <SelectSkeleton hideLabel />
            {:else}
                <ComboBox
                    bind:this={templateRef}
                    on:select={applyTemplate}
                    placeholder={$_("guilds.casedialog.selecttemplate")}
                    items={templates}
                    shouldFilterItem={shouldFilterTemplate}
                    warn={templates.length === 0}
                    disabled={templates.length === 0}
                    warnText={$_("guilds.casedialog.notemplatesfound")} />
            {/if}
        </div>

        <!-- Member -->

        <div class="flex flex-col mb-4">
            <div class="text-lg font-bold mb-2">{$_("guilds.casedialog.member")}</div>
            <div class="text-md mb-2">{$_("guilds.casedialog.memberexplain")}</div>
            {#if membersLoading}
                <SelectSkeleton hideLabel />
            {:else}
                <Autocomplete
                    bind:selectedId={modCase.userId}
                    placeholder={$_("guilds.casedialog.member")}
                    items={members}
                    valueMatchCustomValue={(value) => {
                        return !isNaN(parseFloat(value)) && !isNaN(value - 0);
                    }}
                    warnText={members.length === 0 ? $_("guilds.casedialog.nomembersfound") : ""} />
            {/if}
        </div>

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
                        invalidText={$_("guilds.casedialog.formatisrequired", { values: { format: $currentLanguage?.timeFormat ?? "hh:MM"} })}
                        labelText={$_("guilds.casedialog.punisheduntil")}
                        placeholder={$currentLanguage?.timeFormat ?? "hh:MM"} />
                </div>
            {/if}
        </div>

        <!-- Evidence -->

        <div class="flex flex-col mb-4">
            <div class="text-lg font-bold mb-2">{$_("nav.guild.evidence")}</div>
            <div class="text-md" id="evidence-explained">{$_("guilds.casedialog.evidenceexplained")}</div>
            {#if $linkedEvidence.length > 0}
                <div class="mb-4">
                    <div class="flex flex-col" id="linkedevidence">
                        {#each $linkedEvidence ?? [] as evidence}
                        <Tile class="mb-2">
                            <div class="flex flex-row grow-0 w-full max-w-full items-center">
                                <Box24 class="shrink-0 mr-2"/>
                                <div class="shrink-0 mr-2" style="color: var(--cds-text-02)">
                                    #{evidence.verifiedEvidence.id}
                                </div>
                                <div class="mr-2">
                                    {evidence.verifiedEvidence.username}#{evidence.verifiedEvidence.discriminator}
                                </div>
                                <div class="grow truncate">
                                    {evidence.verifiedEvidence.reportedContent}
                                </div>
                                <div>
                                    <Unlink24 style="cursor: pointer;" on:click={() => unlinkEvidence(evidence)}/>
                                </div>
                            </div>
                        </Tile>
                        {/each}
                    </div>
                </div>
            {/if}
            <Button 
                size="small"
                icon={CopyLink24}
                on:click={onLinkEvidenceButton}
            >
                {$_("guilds.caseview.linkevidence")}
            </Button>
        </div>

        <!-- Files -->

        <div class="flex flex-col mb-4">
            <div class="text-lg font-bold mb-2">{$_("guilds.casedialog.files")}</div>
            <div class="text-md">{$_("guilds.casedialog.filesexplain")}</div>
            <FileUploader bind:files={$filesToUpload} multiple buttonLabel={$_("guilds.casedialog.filesbutton")} status="edit" />
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
            <div class="mr-2">
                <Button
                    kind="secondary"
                    disabled={submitting ||
                        modCase.title === undefined ||
                        modCase.title === null ||
                        (modCase.title?.length ?? 0) === 0 ||
                        (modCase.title?.length ?? 0) > 100 ||
                        modCase.punishmentType === undefined ||
                        modCase.punishmentType === null}
                    on:click={onTemplateModalOpen}>{$_("guilds.casedialog.createtemplate")}</Button>
            </div>
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
                            modCase.punishmentType === null}>{$_("guilds.casedialog.createcase")}</Button>
                {/if}
            </div>
        </div>
    </div>
</MediaQuery>
