<script lang="ts">
    import { writable } from "svelte/store";
    import type { ICompactCaseView } from "../../../models/api/ICompactCaseView";
    import type { IVerifiedEvidenceView } from "../../../models/api/IVerifiedEvidenceView";
    import API from "../../../services/api/api";
    import { currentParams } from "../../../stores/currentParams";
    import { CacheMode } from "../../../services/api/CacheMode";
    import { goto } from "@roxi/routify";
    import { toastError, toastSuccess } from "../../../services/toast/store";
    import { _ } from "svelte-i18n";
    import { Modal, Loading, TextInput, InlineLoading, Tile, SkeletonText, Button, OverflowMenu, OverflowMenuItem, CopyButton } from "carbon-components-svelte";
    import { slide } from "svelte/transition";
    import { Box, List, CopyLink, Share, ChevronUp, ChevronDown, LogoDiscord, TrashCan } from "carbon-icons-svelte";
    import PunishmentTag from "../../api/PunishmentTag.svelte";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import { authUser, isAdminInGuild, isModeratorInGuild } from "../../../stores/auth";
    import type { ICase } from "../../../models/api/ICase";
    import UserIcon from "../../discord/UserIcon.svelte";
    import { currentLanguage } from "../../../stores/currentLanguage";

    let openFurtherDetails = false;

    let loading = true;
    let evidence = writable<IVerifiedEvidenceView>(null);

    let linkToCaseModalOpen = writable(false);
    let linkToCaseSubmitting = writable(false);
    let linkToCaseSearch = writable("");
    let linkToCaseSearching = writable(false);
    let linkToCaseSearchResults = writable<ICompactCaseView[]>([])

    $: $currentParams?.guildId && $currentParams?.evidenceId ? loadData() : null;
    function loadData() {
        loading = true;
        API.get(`/guilds/${$currentParams?.guildId}/evidence/${$currentParams?.evidenceId}/view`, CacheMode.PREFER_CACHE, true)
        .then((response: IVerifiedEvidenceView) => {
            evidence.set(response);
            loading = false;
        })
        .catch(() => {
            $goto("/guilds/" + $currentParams.guildId + "/evidence");
            toastError($_("guilds.evidenceview.evidencenotfound"));
        });
    }

    function onLinkToCaseModalClose() {
        linkToCaseModalOpen.set(false);
        setTimeout(() => {
            linkToCaseSubmitting.set(false);
            linkToCaseSearch.set("");
            linkToCaseSearching.set(false);
            linkToCaseSearchResults.set([]);
        }, 200);
    }

    function linkCase(modCase: ICase) {
        linkToCaseSubmitting.set(true);

        API.post(`/guilds/${$currentParams.guildId}/evidencemapping/${$evidence.evidence.id}/${modCase.caseId}`, {}, CacheMode.API_ONLY, false)
            .then(() => {
                evidence.update((n) => {
                    n.linkedCases.push(modCase);
                    return n;
                });
                toastSuccess($_("guilds.caseview.linked"));
                onLinkToCaseModalClose();
                clearEvidenceCache();
            })
            .catch(() => {
                toastError($_("guilds.caseview.linkfailed"));
            })
            .finally(() => {
                linkToCaseSubmitting.set(false);
            });
    }

    function unlinkCase(caseId: number) {
        console.log("unlinkCase", caseId);
        API.deleteData(`/guilds/${$currentParams.guildId}/evidencemapping/${$evidence.evidence.id}/${caseId}`, {})
            .then(() => {
                evidence.update((n) => {
                    n.linkedCases = n.linkedCases.filter((x) => x.caseId !== caseId);
                    return n;
                });
                toastSuccess($_("guilds.caseview.unlinked"));
                clearEvidenceCache();
            })
            .catch(() => {
                toastError($_("guilds.caseview.unlinkfailed"));
            });
    }

    function clearEvidenceCache() {
        API.clearCacheEntryLike("get", `/guilds/${$currentParams.guildId}/evidence/${$currentParams.evidenceId}`);
        API.clearCacheEntryLike("post", `/guilds/${$currentParams.guildId}/evidencetable`);
    }

    let linkToCaseDebouncer;
    function searchCases(search: string) {
        if (linkToCaseDebouncer) {
            clearTimeout(linkToCaseDebouncer);
        }

        if (search) {
            linkToCaseSearching.set(true);
            linkToCaseSearchResults.set([]);

            const data = {
                customTextFilter: search,
            };

            linkToCaseDebouncer = setTimeout(() => {
                API.post(`/guilds/${$currentParams.guildId}/modcasetable?startPage=0`, data, CacheMode.API_ONLY, false)
                    .then((response: { cases: ICompactCaseView[]; fullSize: number }) => {
                        linkToCaseSearchResults.set(
                            response.cases.filter(
                                (c) => !$evidence.linkedCases.some((x) => x.caseId == c.modCase.caseId)
                            )
                        );
                        linkToCaseSearching.set(false);
                    })
                    .catch(() => {
                        linkToCaseSearching.set(false);
                    });
            }, 500);
        }
    }
    $: searchCases($linkToCaseSearch);

    function shareEvidence() {
        navigator.clipboard
            .writeText(window.location.toString())
            .then(() => {
                toastSuccess($_("guilds.caseview.linkcopied"));
            })
            .catch((e) => console.error(e));
    }

    function deleteEvidence() {
        API.deleteData(`guilds/${$currentParams.guildId}/evidence/${$currentParams.evidenceId}`, null)
        .then(() => {
            API.clearCacheEntryLike("get", `/guilds/${$currentParams.guildId}/evidence/${$currentParams.evidenceId}`);
            API.clearCacheEntryLike("post", `/guilds/${$currentParams.guildId}/evidence/evidencetable`);
            $goto("/guilds/" + $currentParams.guildId + "/evidence");
            toastSuccess($_("guilds.evidenceview.deleted"));
        })
        .catch(() => {
            toastError($_("guilds.evidenceview.deletefailed"))
        })
    }
</script>

<style>
    #title {
        margin-bottom: 1rem;
    }
    #reported-content {
        overflow-wrap: break-word;
    }
</style>

<Modal
    size="sm"
    open={$linkToCaseModalOpen}
    selectorPrimaryFocus="#commentvalue"
    modalHeading={$_("guilds.evidenceview.linktocase")}
    passiveModal
    on:close={onLinkToCaseModalClose}
    >
    <Loading active={$linkToCaseSubmitting}/>
    <div class="mb-2">
        <TextInput
            disabled={$linkToCaseSubmitting}
            labelText={$_("guilds.caseview.search")}
            placeholder={$_("guilds.caseview.search")}
            bind:value={$linkToCaseSearch}
        />
    </div>
    {#if $linkToCaseSearching}
        <div>
            <InlineLoading />
        </div>
    {:else}
        <div class="flex flex-col" transition:slide|local>
            {#each $linkToCaseSearchResults as modCase}
                <div transition:slide|local>
                    <Tile class="mb-2" light>
                        <div class="flex flex-row grow-0 w-full max-w-full items-center">
                            <List class="shrink-0 mr-2"/>
                            <div class="shrink-0 mr-2" style="color: var(--cds-text-02)">
                                #{modCase.modCase.caseId}
                            </div>
                            <div class="grow truncate">
                                {modCase.modCase.title}
                            </div>
                            <div class="grow" />
                            <div class="shrink-0">
                                <PunishmentTag modCase={modCase.modCase} />
                            </div>
                            <div class="cursor-pointer">
                                <Button icon={CopyLink} kind="ghost" iconDescription="link" class="mr-2" on:click={() => linkCase(modCase.modCase)} />
                            </div>
                        </div>
                    </Tile>
                </div>
            {:else}
                {#if $linkToCaseSearch}
                    {$_("guilds.caseview.nocasesfound")}
                {/if}
            {/each}
        </div>
    {/if}
</Modal>

<MediaQuery query="(min-width: 1024px)" let:matches>
    <div class="flex flex-col">
        <!-- Header -->
        <div class="flex flex-col">
            <!-- Icon -->
            <div class="flex flex-row grow items-center" id="title" style="color: var(--cds-text-02)">
                <Box class="mr-2" />
                {#if $currentParams?.guild?.name && $currentParams?.evidenceId && !loading}
                    <div class="flex flex-row items-center">
                        {$currentParams.guild?.name}-{$_("guilds.evidenceview.evidence")}#{$currentParams.evidenceId}
                    </div>
                {:else}
                    <SkeletonText class="!mb-0" width="100px" />
                {/if}
            </div>
            <!-- Title -->
            <div class="flex flex-col mb-4">
                <div class="flex flex-row flex-wrap items-center">
                    {#if loading}
                        <div class="mr-2">
                            <Button size="small" skeleton />
                        </div>
                    {:else}
                        <div class:mr-2={matches}>
                            <Button size="small" kind="secondary" icon={Share} on:click={shareEvidence}>{$_("guilds.evidenceview.shareevidence")}</Button>
                        </div>
                        {#if matches}
                            <div class="mr-2">
                                <Button
                                    size="small"
                                    kind="secondary"
                                    icon={LogoDiscord}
                                    href={`discord://discord.com/channels/${$evidence.evidence.guildId}/${$evidence.evidence.channelId}/${$evidence.evidence.messageId}`}>
                                        {$_("guilds.evidenceview.goto")}
                                </Button>
                            </div>
                            {#if isModeratorInGuild($authUser, $currentParams.guildId)}
                                <div class="mr-2">
                                    <Button
                                        size="small"
                                        kind="secondary"
                                        icon={CopyLink}
                                        on:click={() => linkToCaseModalOpen.set(true)}>
                                            {$_("guilds.evidenceview.linktocase")}
                                        </Button>
                                </div>
                            {/if}
                            {#if isAdminInGuild($authUser, $currentParams.guildId)}
                                <div class="mr-2">
                                    <Button
                                        size="small"
                                        kind="danger"
                                        icon={TrashCan}
                                        on:click={deleteEvidence}>
                                            {$_("guilds.caseview.delete")}
                                    </Button>
                                </div>
                            {/if}
                        {:else}
                            <div class="grow" />
                            <OverflowMenu flipped>
                                <OverflowMenuItem
                                    text={$_("guilds.evidenceview.goto")}
                                    href={`discord://discord.com/channels/${$evidence.evidence.guildId}/${$evidence.evidence.channelId}/${$evidence.evidence.messageId}`}/>
                                {#if isModeratorInGuild($authUser, $currentParams.guildId)}
                                    <OverflowMenuItem
                                        text={$_("guilds.evidenceview.linktocase")}
                                        on:click={() => linkToCaseModalOpen.set(true)}/>
                                {/if}
                                {#if isAdminInGuild($authUser, $currentParams.guildId)}
                                    <OverflowMenuItem
                                        text={$_("guilds.caseview.delete")}
                                        on:click={deleteEvidence}/>
                                {/if}
                            </OverflowMenu>
                        {/if}
                    {/if}
                </div>
            </div>
        </div>
        <div class="flex flex-col lg:flex-row grow">
            <!-- Content -->
            <div class="flex flex-col grow shrink-0 {matches ? 'pr-4 w-2/3' : ''}">
                {#if loading}
                    <SkeletonText paragraph />
                {:else}
                    <div class="text-sm mb-4" id="reported-content">
                        <h4>{$_("guilds.evidenceview.reportedcontent")}:</h4>
                        {$evidence?.evidence?.reportedContent ?? ''}
                    </div>
                {/if}
                <!-- Linked Cases -->
                {#if loading}
                    <div class="flex flex-row">
                        <SkeletonText width={"30%"} />
                        <div class="grow" />
                        <SkeletonText width={"5%"} />
                    </div>
                    <div class="flex flex-col">
                        <div class="m-2">
                            <SkeletonText />
                        </div>
                        <div class="m-2">
                            <SkeletonText />
                        </div>
                        <div class="m-2">
                            <SkeletonText />
                        </div>
                    </div>
                {:else if ($evidence?.linkedCases?.length ?? 0) !== 0}
                    <div class="mb-4">
                        <div class="flex flex-row mb-2">
                            <div class="font-bold">Linked cases</div>
                        </div>
                        <div class="flex flex-col" id="linkedcases">
                            {#each $evidence?.linkedCases ?? [] as linked}
                                <Tile class="mb-2">
                                    <div class="flex flex-row grow-0 w-full max-w-full items-center">
                                        <List class="shrink-0 mr-2" />
                                        <div class="shrink-0 mr-2" style="color: var(--cds-text-02)">
                                            #{linked.caseId}
                                        </div>
                                        <div class="grow truncate">
                                            {linked.title}
                                        </div>
                                        <div class="grow" />
                                        <div class="shrink-0">
                                            <PunishmentTag modCase={linked} />
                                        </div>
                                        <div>
                                            <OverflowMenu flipped>
                                                <OverflowMenuItem
                                                    text="Show case"
                                                    href={`/guilds/${$currentParams.guildId}/cases/${linked.caseId}`} />
                                                {#if isModeratorInGuild($authUser, $currentParams.guildId)}
                                                    <OverflowMenuItem
                                                        danger
                                                        text="Unlink case"
                                                        on:click={() => {
                                                            unlinkCase(linked.caseId);
                                                        }} />
                                                {/if}
                                            </OverflowMenu>
                                        </div>
                                    </div>
                                </Tile>
                            {/each}
                        </div>
                    </div>
                {/if}
            </div>
            <!-- Meta data -->
            <div class="flex flex-col grow shrink-0 {matches ? 'pl-4 w-1/3' : ''}" class:-order-1={!matches}>
                {#if loading}
                    <div class="flex flex-row lg:flex-col">
                        <SkeletonText width={"10%"} />
                        <SkeletonText width={"30%"} />
                    </div>
                    <div class="flex flex-row lg:flex-col">
                        <SkeletonText width={"10%"} />
                        <SkeletonText width={"30%"} />
                    </div>
                {:else}
                    {#if !matches}
                        <div class="flex flex-row justify-end mb-6">
                            <Button
                                id="furtherdetails-case-button"
                                size="small"
                                iconDescription=""
                                icon={openFurtherDetails ? ChevronUp : ChevronDown}
                                on:click={() => openFurtherDetails = !openFurtherDetails} />
                        </div>
                    {/if}
                    {#if matches || openFurtherDetails}
                        <div class="flex flex-col grow shrink-0" transition:slide|local>
                            <div class="flex flex-col mb-6">
                                <div class="font-bold mb-2">{$_("guilds.caseview.violator")}</div>
                                <div class="flex flex-row items-center">
                                    <UserIcon class="mr-2" user={$evidence.reported} />
                                    <div class="flex flex-row flex-wrap items-center">
                                        <div class="mr-2">
                                            {$evidence.reported?.username ?? $evidence.evidence.username}
                                        </div>
                                        <div class="mr-2" style="color: var(--cds-text-02)">
                                            ({$evidence.evidence.userId})
                                        </div>
                                    </div>
                                    <div class="grow" />
                                    <div>
                                        <CopyButton text={$evidence.evidence.userId} feedback={$_("core.copiedtoclipboard")} />
                                    </div>
                                </div>
                            </div>
                            <div class="flex flex-col mb-6">
                                <div class="font-bold mb-2">{$_("guilds.caseview.moderator")}</div>
                                {#if $evidence.evidence.modId}
                                    <div class="flex flex-row items-center">
                                        <UserIcon class="mr-2" user={$evidence.moderator} />
                                        <div class="flex flex-row flex-wrap">
                                            {#if $evidence.moderator}
                                                <div class="mr-2">
                                                    {$evidence.moderator?.username}
                                                </div>
                                            {/if}
                                            <div class="mr-2" style="color: var(--cds-text-02)">
                                                ({$evidence.evidence.modId})
                                            </div>
                                        </div>
                                        <div class="grow" />
                                        <div>
                                            <CopyButton text={$evidence.evidence.modId} feedback={$_("core.copiedtoclipboard")} />
                                        </div>
                                    </div>
                                {:else}
                                    {$_("guilds.caseview.moderatorunknown")}
                                {/if}
                            </div>
                            <hr class="mb-6" style="border-color: var(--cds-ui-04)" />
                            <div class="flex flex-row mb-2" style="color: var(--cds-text-02)">
                                <div class="mr-2">{$_("guilds.evidenceview.sent")}</div>
                                <div>
                                    {$evidence.evidence.sentAt?.format($currentLanguage?.momentDateTimeFormat ?? "MMMM Do YYYY, h:mm:ss") ??
                                        $_("guilds.caseview.unknown")}
                                </div>
                            </div>
                            <div class="flex flex-row mb-2" style="color: var(--cds-text-02)">
                                <div class="mr-2">{$_("guilds.evidenceview.reported")}</div>
                                <div>
                                    {$evidence.evidence.reportedAt?.format($currentLanguage?.momentDateTimeFormat ?? "MMMM Do YYYY, h:mm:ss") ??
                                        $_("guilds.caseview.unknown")}
                                </div>
                            </div>
                        </div>
                    {/if}
                {/if}
            </div>
        </div>
    </div>
</MediaQuery>