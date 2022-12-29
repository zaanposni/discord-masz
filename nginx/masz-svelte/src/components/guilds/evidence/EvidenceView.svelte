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
    import { Modal, Loading, TextInput, InlineLoading, Tile, SkeletonText, Button, OverflowMenu, OverflowMenuItem } from "carbon-components-svelte";
    import { slide } from "svelte/transition";
    import { List24, CopyLink24, Share24 } from "carbon-icons-svelte";
    import PunishmentTag from "../../api/PunishmentTag.svelte";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import { authUser, isModeratorInGuild } from "../../../stores/auth";

    let loading = true;
    let evidence = writable<IVerifiedEvidenceView>(null);
    let renderedContent = "";

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
            renderContent(response?.evidence?.reportedContent ?? "");
            loading = false;
        })
        .catch(() => {
            $goto("/guilds/" + $currentParams.guildId + "/evidence");
            toastError($_("guilds.evidenceview.evidencenotfound"));
        });
    }

    function renderContent(content: string) {
        let value = content.replace("<", "&lt;").replace(">", "&gt;").replace(/\n/g, "<br>");
        value = value.replace(/#([\d]+)/g, (match) => {
            const id = match.substring(1);
            if (id == $evidence.evidence.discriminator || id == ($evidence.reported?.discriminator ?? '')) {
                return match;
            }
            return `<a href=/guilds/${$currentParams.guildId}/evidence/${id}>#${id}</a>`
        });
        renderedContent = value;
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

    function shareEvidence() {
        navigator.clipboard
            .writeText(window.location.toString())
            .then(() => {
                toastSuccess($_("guilds.caseview.linkcopied"));
            })
            .catch((e) => console.error(e));
    }
</script>

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
                            <List24 class="shrink-0 mr-2"/>
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
                            <!-- <div class="cursor-pointer">
                                <CopyLink24 class="mr-2" on:click={() => linkCase(modCase.modCase)} />
                            </div> -->
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
            <div class="flex flex-row grow items-center" style="color: var(--cds-text-02)">
                <List24 class="mr-2" />
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
                        <div class="mr-2 mb-2">
                            <Button size="small" skeleton />
                        </div>
                    {:else}
                        <div class:mb-2={matches && isModeratorInGuild($authUser, $currentParams.guildId)} class:mr-2={matches}>
                            <Button size="small" kind="secondary" icon={Share24} on:click={shareEvidence}>{$_("guilds.evidenceview.shareevidence")}</Button>
                        </div>
                        {#if matches}
                            {#if isModeratorInGuild($authUser, $currentParams.guildId)}
                                <div class="mr-2 mb-2">
                                    <Button
                                        size="small"
                                        kind="secondary"
                                        icon={CopyLink24}
                                        on:click={() => linkToCaseModalOpen.set(true)}
                                        >
                                            {$_("guilds.evidenceview.linktocase")}
                                        </Button>
                                </div>
                            {/if}
                        {:else}
                            <div class="grow" />
                            <OverflowMenu flipped>
                                <OverflowMenuItem
                                    text={$_("guilds.evidenceview.linktocase")}
                                    on:click={() => linkToCaseModalOpen.set(true)}
                                />
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
                    <div class="text-sm mb-4" id="casedescription">
                        <h2>{$_("guilds.evidenceview.reportedcontent")}:</h2>
                        {@html renderedContent}
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
                {:else if isModeratorInGuild($authUser, $currentParams.guildId) && ($evidence?.linkedCases?.length ?? 0) !== 0}
                    <div class="mb-4">
                        <div class="flex flex-row mb-2">
                            <div class="font-bold">{$_("guilds.evidenceview.linkedcases")}</div>
                        </div>
                        <div class="flex flex-col" id="linkedcases">
                            {#each $evidence?.linkedCases ?? [] as linked}
                                <Tile class="mb-2">
                                    <div class="flex flex-row grow-0 w-full max-w-full items-center">
                                        <List24 class="shrink-0 mr-2" />
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
                                                <!-- <OverflowMenuItem
                                                    danger
                                                    text="Unlink case"
                                                    on:click={() => {
                                                        unlinkCase(linked.caseId);
                                                    }} /> -->
                                            </OverflowMenu>
                                        </div>
                                    </div>
                                </Tile>
                            {/each}
                        </div>
                    </div>
                {/if}
            </div>
        </div>
    </div>
</MediaQuery>