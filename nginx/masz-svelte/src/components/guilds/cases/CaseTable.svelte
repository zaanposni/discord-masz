<script lang="ts">
    import { Launch20, Add32, Filter24, Search32 } from "carbon-icons-svelte";
    import { Button, ComboBox, Link, MultiSelect, SkeletonPlaceholder, SkeletonText, Tag, TextInput, Tile } from "carbon-components-svelte";
    import type { ICompactCaseView } from "../../../models/api/ICompactCaseView";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import PunishmentTag from "../../api/PunishmentTag.svelte";
    import UserIcon from "../../discord/UserIcon.svelte";
    import { currentParams } from "./../../../stores/currentParams";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import { Pagination } from "carbon-components-svelte";
    import { PaginationSkeleton } from "carbon-components-svelte";
    import { authUser } from "../../../stores/auth";
    import { url } from "@roxi/routify";
    import { slide } from "svelte/transition";
    import CaseCreationType from "../../../services/enums/CaseCreationType";
    import { _ } from "svelte-i18n";
    import PunishmentType from "../../../services/enums/PunishmentType";
    import EditStatus from "../../../services/enums/EditStatus";
    import LockedCommentStatus from "../../../services/enums/LockedCommentStatus";
    import MarkedToDeleteStatus from "../../../services/enums/MarkedToDeleteStatus";
    import PunishmentActiveStatus from "../../../services/enums/PunishmentActiveStatus";
    import type { IDiscordUser } from "../../../models/discord/IDiscordUser";
    import Warning_02 from "carbon-pictograms-svelte/lib/Warning_02.svelte";
    import { toastError } from "../../../services/toast/store";

    let cases: ICompactCaseView[] = [];
    let initialLoading: boolean = true;
    let loading: boolean = true;
    let fullSize: number = 0;
    let currentPage: number = 1;

    let members: { id: string; text: string }[] = [];
    let filterOpened: boolean = false;
    let filter: any = {};
    let enums = {};

    $: forwardText = $_("core.pagination.forwardtext");
    $: backwardText = $_("core.pagination.backwardtext");
    $: itemRangeText = (min, max, total) => $_("core.pagination.itemrangetext", { values: { min, max, total } });
    $: pageRangeText = (current, total) => $_(`core.pagination.pagerangetext${total === 1 ? "" : "plural"}`, { values: { total } });

    $: enums = {
        casecreationtype: CaseCreationType.getAll().map((x) => ({
            id: x.id.toString(),
            text: $_(x.translationKey),
        })),
        punishmenttype: PunishmentType.getAll().map((x) => ({
            id: x.id.toString(),
            text: $_(x.translationKey),
        })),
        editstatus: EditStatus.getAll().map((x) => ({
            id: x.id.toString(),
            text: $_(x.translationKey),
        })),
        lockedCommentStatus: LockedCommentStatus.getAll().map((x) => ({
            id: x.id.toString(),
            text: $_(x.translationKey),
        })),
        deleteStatus: MarkedToDeleteStatus.getAll().map((x) => ({
            id: x.id.toString(),
            text: $_(x.translationKey),
        })),
        activeStatus: PunishmentActiveStatus.getAll().map((x) => ({
            id: x.id.toString(),
            text: $_(x.translationKey),
        })),
    };

    let lastUsedGuildId: string = "";
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
        let filterIsSet = Object.keys(filter).length > 0;
        API.post(
            `/guilds/${$currentParams.guildId}/modcasetable?startPage=${page - 1}`,
            filter,
            filterIsSet ? CacheMode.API_ONLY : CacheMode.PREFER_CACHE,
            !filterIsSet
        )
            .then((response: { cases: ICompactCaseView[]; fullSize: number }) => {
                cases = response.cases;
                fullSize = response.fullSize;
                loading = false;
                initialLoading = false;
            })
            .catch(() => {
                loading = false;
                initialLoading = false;
                toastError($_("guilds.modcasetable.failedtoload"));
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
        cases = [];
        fullSize = 0;
        currentPage = 1;
        loadData(currentPage);
    }

    function toggleFilter() {
        filterOpened = !filterOpened;
        filter = {};
    }
</script>

<style>
    a {
        color: unset;
        text-decoration: unset;
    }

    :global(.case-label-list .bx--tag:not(:first-child)) {
        margin-left: 0.25rem;
    }
    :global(.case-label-list .bx--tag) {
        margin-right: 0;
    }
</style>

<MediaQuery query="(min-width: 768px)" let:matches>
    <!-- Header -->
    <div class="flex flex-col mb-4">
        <h2 class="font-weight-bold mb-4">
            {$_("nav.guild.cases")}
        </h2>
        <div class="flex flex-row">
            {#if $authUser?.isAdmin || $authUser?.adminGuilds?.find((x) => x.id === $currentParams.guildId) || $authUser?.modGuilds?.find((x) => x.id === $currentParams.guildId)}
                <Button class="!mr-2" icon={Add32} href={$url(`/guilds/${$currentParams.guildId}/cases/new`)}
                    >{$_("guilds.modcasetable.createnewcase")}</Button>
            {/if}
            <Button iconDescription={$_("guilds.modcasetable.useadvancedfilter")} icon={Filter24} on:click={toggleFilter} />
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
                        titleText={$_("guilds.modcasetable.selectmembers")}
                        label={$_("guilds.modcasetable.selectmembers")}
                        items={members}
                        on:clear={() => onSelect("userIds", [])}
                        on:select={(e) => onSelect("userIds", e.detail.selectedIds)} />
                </div>
                <div>
                    <MultiSelect
                        spellcheck="false"
                        filterable
                        titleText={$_("guilds.modcasetable.selectmoderators")}
                        label={$_("guilds.modcasetable.selectmoderators")}
                        items={members}
                        on:clear={() => onSelect("moderatorIds", [])}
                        on:select={(e) => onSelect("moderatorIds", e.detail.selectedIds)} />
                </div>
                <div>
                    <MultiSelect
                        titleText={$_("guilds.modcasetable.selectcreationtypes")}
                        label={$_("guilds.modcasetable.selectcreationtypes")}
                        items={enums["casecreationtype"]}
                        on:clear={() => onSelect("creationTypes", [])}
                        on:select={(e) => onSelect("creationTypes", e.detail.selectedIds)} />
                </div>
                <div>
                    <MultiSelect
                        titleText={$_("guilds.modcasetable.selectpunishments")}
                        label={$_("guilds.modcasetable.selectpunishments")}
                        items={enums["punishmenttype"]}
                        on:clear={() => onSelect("punishmentTypes", [])}
                        on:select={(e) => onSelect("punishmentTypes", e.detail.selectedIds)} />
                </div>
                <div>
                    <ComboBox
                        titleText={$_("guilds.modcasetable.selectedited")}
                        placeholder={$_("guilds.modcasetable.selectedited")}
                        items={enums["editstatus"]}
                        on:clear={() => onSelect("edited", undefined)}
                        on:select={(e) => onSelect("edited", e.detail.selectedId === "0" ? undefined : e.detail.selectedId !== "1")} />
                </div>
                <div>
                    <ComboBox
                        titleText={$_("guilds.modcasetable.selectlocked")}
                        placeholder={$_("guilds.modcasetable.selectlocked")}
                        items={enums["editstatus"]}
                        on:clear={() => onSelect("lockedComments", undefined)}
                        on:select={(e) => onSelect("lockedComments", e.detail.selectedId === "0" ? undefined : e.detail.selectedId !== "1")} />
                </div>
                <div>
                    <ComboBox
                        titleText={$_("guilds.modcasetable.selectdelete")}
                        placeholder={$_("guilds.modcasetable.selectdelete")}
                        items={enums["deleteStatus"]}
                        on:clear={() => onSelect("markedToDelete", undefined)}
                        on:select={(e) => onSelect("markedToDelete", e.detail.selectedId === "0" ? undefined : e.detail.selectedId !== "1")} />
                </div>
                <div>
                    <ComboBox
                        titleText={$_("guilds.modcasetable.selectactive")}
                        placeholder={$_("guilds.modcasetable.selectactive")}
                        items={enums["activeStatus"]}
                        on:clear={() => onSelect("punishmentActive", undefined)}
                        on:select={(e) => onSelect("punishmentActive", e.detail.selectedId === "0" ? undefined : e.detail.selectedId !== "1")} />
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
    <!-- Table -->
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
            {#each cases as caseView}
                <a href={$url(`/guilds/${caseView.modCase.guildId}/cases/${caseView.modCase.caseId}`)}>
                    <Tile class="cursor-pointer mb-2">
                        <div class="flex flex-row items-center">
                            <div class="grow flex-shrink">
                                <div class="flex flex-col grow flex-shrink">
                                    <div class="flex flex-row">
                                        <UserIcon
                                            size={matches ? 48 : 32}
                                            class="{matches ? 'self-center' : 'self-start'} mr-3"
                                            user={caseView.suspect} />
                                        <div class="flex flex-col flex-shrink">
                                            <div class="flex flex-row items-center">
                                                <PunishmentTag class="grow-0 shrink-0 !ml-0 mr-1" modCase={caseView.modCase} />
                                                <div class="font-bold" style="word-wrap: anywhere">
                                                    {caseView.suspect?.username ?? caseView.modCase.username}#{caseView.suspect?.discriminator ??
                                                        caseView.modCase.discriminator}
                                                </div>
                                            </div>
                                            <div class="font-bold" style="word-wrap: anywhere" title={caseView.modCase.title}>
                                                #{caseView.modCase.caseId} - {caseView.modCase.title.length > 100
                                                    ? caseView.modCase.title.substring(0, 100) + " [...]"
                                                    : caseView.modCase.title}
                                            </div>
                                        </div>
                                    </div>
                                    {#if caseView.modCase.labels}
                                        <div class="flex flex-row flex-wrap items-center mt-2 case-label-list">
                                            {#each caseView.modCase.labels as label}
                                                <Tag type="outline">{label}</Tag>
                                            {/each}
                                        </div>
                                    {/if}
                                    <div class="mt-2">
                                        <div title={caseView.modCase.description} style="word-wrap: anywhere">
                                            {caseView.modCase.description.length > (matches ? 1000 : 200)
                                                ? caseView.modCase.description.substring(0, matches ? 1000 : 200) + " [...]"
                                                : caseView.modCase.description}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <Link href={`/guilds/${caseView.modCase.guildId}/cases/${caseView.modCase.caseId}`} icon={Launch20} class="align-end" />
                        </div>
                    </Tile>
                </a>
            {/each}
        </div>
    {/if}
</MediaQuery>
