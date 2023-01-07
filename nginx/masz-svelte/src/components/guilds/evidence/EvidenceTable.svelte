<script lang="ts">
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import { _ } from "svelte-i18n";
    import { MultiSelect, TextInput, Button, SkeletonPlaceholder, SkeletonText, Tile, Link } from "carbon-components-svelte";
    import { Search32, Launch20, Filter24, Add24 } from "carbon-icons-svelte";
    import { slide } from "svelte/transition";
    import { currentParams } from "../../../stores/currentParams";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import type { IVerifiedEvidenceCompactView } from "../../../models/api/IVerifiedEvidenceCompactView";
    import { toastError, toastSuccess } from "../../../services/toast/store";
    import type { IDiscordUser } from "../../../models/discord/IDiscordUser";
    import { Pagination } from "carbon-components-svelte";
    import { PaginationSkeleton } from "carbon-components-svelte";
    import Warning_02 from "carbon-pictograms-svelte/lib/Warning_02.svelte";
    import UserIcon from "../../discord/UserIcon.svelte";
    import { url } from "@roxi/routify";
    import { writable } from "svelte/store";
    import { currentLanguage } from "../../../stores/currentLanguage";
    import CreateEvidenceModal from "./CreateEvidenceModal.svelte";

    let evidence: IVerifiedEvidenceCompactView[] = [];
    let loading = true;
    let initialLoading = true;
    let currentPage = 1;
    let fullSize = 0;

    let createModalOpen = writable(false);

    let filterOpened = false;
    let filter: any = {};
    let members: { id: string; text: string }[] = [];

    $: forwardText = $_("core.pagination.forwardtext");
    $: backwardText = $_("core.pagination.backwardtext");
    $: itemRangeText = (min, max, total) => $_("core.pagination.itemrangetext", { values: { min, max, total } });
    $: pageRangeText = (current, total) => $_(`core.pagination.pagerangetext${total === 1 ? "" : "plural"}`, { values: { total } });

    let lastUsedGuildId = "";
    $: $currentParams?.guildId && currentPage ? loadData(currentPage) : null;
    function loadData(page: number = 1) {
        loading = true;
        if (lastUsedGuildId !== "" && lastUsedGuildId !== $currentParams?.guildId) {
            // reset on guild change
            currentPage = 1;
            page = 1;
            filterOpened = false;
            filter = {};
            searchValue = "";
        }
        lastUsedGuildId = $currentParams?.guildId;
        const filterIsSet = Object.keys(filter).length > 0;
        API.post(
            `/guilds/${$currentParams.guildId}/evidence/evidencetable?startPage=${page - 1}`,
            filter,
            filterIsSet ? CacheMode.API_ONLY : CacheMode.PREFER_CACHE,
            !filterIsSet
            )
            .then((response: {evidence: IVerifiedEvidenceCompactView[]; fullSize: number}) => {
                evidence = response.evidence;
                fullSize = response.fullSize;
                loading = false;
                initialLoading = false;
            })
            .catch(() => {
                loading = false;
                initialLoading = false;
                toastError($_("guilds.evidencetable.failedtoload"));
            });
    }

    $: $currentParams?.guildId ? loadGuildData() : null;
    function loadGuildData() {
        API.get(`/discord/guilds/${$currentParams.guildId}/members`, CacheMode.PREFER_CACHE, true).then((response: IDiscordUser[]) => {
            members = response.map((x) => ({
                id: x.id,
                text: `${x.username}#${x.discriminator}`,
            }));
        });
    }

    function onSelect(prop: string, value: any) {
        if (Array.isArray(value)) {
            if (value.length === 0) {
                delete filter[prop];
            } else {
                filter[prop] = value;
            }
        } else {
            if (value === undefined) {
                delete filter[prop];
            } else {
                filter[prop] = value;
            }
        }
    }

    let searchValue = "";
    $: onSearchChange(searchValue);
    function onSearchChange(value: string) {
        if (value.length === 0) {
            delete filter.customTextFilter;
        } else {
            filter.customTextFilter = value;
        }
    }

    function executeSearch() {
        loading = true;
        currentPage = 1;
        loadData(currentPage);
    }

    function toggleFilter() {
        filterOpened = !filterOpened;
        filter = {};
    }

    function onEvidenceCreated() {
        loadData(currentPage);
    }
</script>

<style>
    a {
        color: unset;
        text-decoration: unset;
    }
</style>

<CreateEvidenceModal bind:open={$createModalOpen} on:create={onEvidenceCreated} />

<MediaQuery query="(min-width: 768px)" let:matches>
    <div class="flex flex-col mb-4">
        <h2 class="font-weight-bold mb-4">
            {$_("nav.guild.evidence")}
        </h2>
        <div class="flex flex-row">
            <Button iconDescription={$_("guilds.modcasetable.useadvancedfilter")} icon={Filter24} on:click={toggleFilter} />
            <Button iconDescription={$_("guilds.evidencetable.create")} icon={Add24} on:click={() => createModalOpen.set(!$createModalOpen)}/>
        </div>
        {#if filterOpened}
            <div
                class="grid gap-1 grid-cols-1 md:grid-cols-2 lg:grid-cols-4 xl:grid-cols-6 3xl:grid-cols-12 mt-4"
                id="filter-parent"
                transition:slide|local>
                <div>
                    <MultiSelect
                        spellcheck="false"
                        filterable
                        titleText={$_("guilds.modcasetable.selectmoderators")}
                        label={$_("guilds.modcasetable.selectmoderators")}
                        items={members}
                        on:clear={() => onSelect("modIds", [])}
                        on:select={(e) => onSelect("modIds", e.detail.selectedIds)} />
                </div>
                <div>
                    <MultiSelect
                        spellcheck="false"
                        filterable
                        titleText={$_("guilds.modcasetable.selectmembers")}
                        label={$_("guilds.modcasetable.selectmembers")}
                        items={members}
                        on:clear={() => onSelect("reportedIds", [])}
                        on:select={(e) => onSelect("reportedIds", e.detail.selectedIds)} />
                </div>
                <div>
                    <TextInput bind:value={searchValue} labelText={$_("guilds.modcasetable.search")} placeholder={$_("guilds.modcasetable.search")} />
                </div>
                <div class="self-end">
                    <Button icon={Search32} on:click={executeSearch}>{$_("guilds.modcasetable.executesearch")}</Button>
                </div>
            </div>
        {/if}
    </div>
    {#if initialLoading}
        <PaginationSkeleton class="mb-4" />
    {:else}
        <Pagination
            class="mb-4"
            disabled={loading}
            bind:page={currentPage}
            pageSize={20}
            totalItems={fullSize}
            pageSizeInputDisabled
            {forwardText}
            {backwardText}
            {itemRangeText}
            {pageRangeText} />
    {/if}
    {#if loading}
        <div class="grid gap-1 grid-cols-1">
            {#each { length: 3 } as _, i}
                <Tile class="mb-2">
                    <div class="flex flex-row items-center">
                        <div class="grow flex-shrink">
                            <div class="flex flex-col grow flex-shrink">
                                <div class="flex flex-row">
                                    <SkeletonPlaceholder class="mr-3" style="height: 4rem; width: 4rem" />
                                    <div class="flex grow flex-col flex-shrink">
                                        <div class="flex flex-row items-center">
                                            <SkeletonText width={"5%"} />
                                            <div class="mr-5" />
                                            <SkeletonText width={"20%"} />
                                        </div>
                                        <SkeletonText width={"50%"} />
                                    </div>
                                </div>
                                <SkeletonText paragraph lines={2} class="mt-2" />
                            </div>
                        </div>
                    </div>
                </Tile>
            {/each}
        </div>
    {:else if fullSize === 0}
        <div class="flex flex-col ml-2" class:items-center={!matches}>
            <Warning_02 />
            <div class="text-lg font-bold">{$_("guilds.modcasetable.nomatches")}</div>
            <div class="text-md">{$_("guilds.modcasetable.nomatchesdescription")}</div>
        </div>
    {:else}
        <div class="grid gap-1 grid-cols-1">
            {#each evidence as evidenceView}
                <a href={$url(`/guilds/${evidenceView.verifiedEvidence.guildId}/evidence/${evidenceView.verifiedEvidence.id}`)}>
                    <Tile class="cursor-pointer mb-2">
                        <div class="flex flex-row items-center">
                            <div class="grow flex-shrink">
                                <div class="flex flex-col grow flex-shrink">
                                    <div class="flex flex-row">
                                        <UserIcon
                                            size={matches ? 48 : 32}
                                            class="{matches ? 'self-center' : 'self-start'} mr-3"
                                            user={evidenceView.reported} />
                                        <div class="flex flex-col flex-shrink">
                                            <div class="flex flex-row items-center">
                                                <div class="font-bold" style="word-wrap: anywhere">
                                                    {evidenceView.reported?.username ?? evidenceView.reported.username}#{evidenceView.reported?.discriminator ??
                                                        evidenceView.verifiedEvidence.discriminator}
                                                </div>
                                            </div>
                                            <div class="mt-1">
                                                {evidenceView.verifiedEvidence.sentAt?.format($currentLanguage?.momentDateTimeFormat ?? "MMMM Do YYYY, h:mm:ss") ?? $_("guilds.evidenceview.unknown")}
                                            </div>
                                        </div>
                                    </div>
                                    <div class="mt-2">
                                        <div title={evidenceView.verifiedEvidence.reportedContent} style="word-wrap: anywhere">
                                            {evidenceView.verifiedEvidence.reportedContent.length > (matches ? 1000 : 200)
                                                ? evidenceView.verifiedEvidence.reportedContent.substring(0, matches ? 1000 : 200) + " [...]"
                                                : evidenceView.verifiedEvidence.reportedContent}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <Link href={`/guilds/${evidenceView.verifiedEvidence.guildId}/evidence/${evidenceView.verifiedEvidence.id}`} icon={Launch20} class="align-end" />
                        </div>
                    </Tile>
                </a>
            {/each}
        </div>
    {/if}

</MediaQuery>