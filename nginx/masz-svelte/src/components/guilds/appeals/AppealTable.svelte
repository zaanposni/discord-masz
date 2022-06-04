<script lang="ts">
    import { Launch20, Add32, Filter24, Search32, Settings32 } from "carbon-icons-svelte";
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
    import AppealStatus from "../../../services/enums/AppealStatus";
    import { _ } from "svelte-i18n";
    import type { IDiscordUser } from "../../../models/discord/IDiscordUser";
    import Warning_02 from "carbon-pictograms-svelte/lib/Warning_02.svelte";
    import { toastError } from "../../../services/toast/store";
    import type { IAppealTable } from "../../../models/api/IAppealTable";
    import type { IAppealView } from "../../../models/api/IAppealView";
    import AppealStatusTag from "../../api/AppealStatusTag.svelte";
    import { currentLanguage } from "../../../stores/currentLanguage";
    import type { Writable } from "svelte/store";
    import { writable } from "svelte/store";

    const allowedToCreateAppeal: Writable<boolean> = writable(false);
    let appeals: IAppealView[] = [];
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
        appealstatus: AppealStatus.getAll().map((x) => ({
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
            allowedToCreateAppeal.set(false);
        }

        lastUsedGuildId = $currentParams?.guildId;
        let filterIsSet = Object.keys(filter).length > 0;

        API.post(
            `/guilds/${$currentParams.guildId}/appeal/table?startPage=${page - 1}`,
            filter,
            filterIsSet ? CacheMode.API_ONLY : CacheMode.PREFER_CACHE,
            !filterIsSet
        )
            .then((response: IAppealTable) => {
                appeals = response.appealViews;
                fullSize = response.fullSize;
                loading = false;
                initialLoading = false;
            })
            .catch(() => {
                loading = false;
                initialLoading = false;
                toastError($_("guilds.appealtable.failedtoload"));
            });

        API.get(`/guilds/${$currentParams.guildId}/appeal/allowed`, CacheMode.API_ONLY, false).then((response: { allowed: boolean }) => {
            allowedToCreateAppeal.set(response.allowed);
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
        appeals = [];
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
</style>

<MediaQuery query="(min-width: 768px)" let:matches>
    <!-- Header -->
    <div class="flex flex-col mb-4">
        <h2 class="font-weight-bold mb-4">
            {$_("nav.guild.appeals")}
        </h2>
        <div class="flex flex-row">
            {#if $authUser?.isAdmin || $authUser?.adminGuilds?.find((x) => x.id === $currentParams.guildId)}
                <Button class="!mr-2" icon={Settings32} href={$url(`/guilds/${$currentParams.guildId}/appeals/config`)}
                    >{$_("guilds.appealtable.configureappeal")}</Button>
            {:else if !$authUser?.memberGuilds?.find((x) => x.id === $currentParams.guildId) && !$authUser?.modGuilds?.find((x) => x.id === $currentParams.guildId)}
                <Button
                    class="!mr-2"
                    icon={Add32}
                    href={$url(`/guilds/${$currentParams.guildId}/appeals/new`)}
                    disabled={!$allowedToCreateAppeal}
                    title={!$allowedToCreateAppeal ? $_("guilds.appealtable.disabledexplained") : null}
                    >{$_("guilds.appealtable.createnewappeal")}</Button>
            {/if}
            <Button iconDescription={$_("guilds.appealtable.useadvancedfilter")} icon={Filter24} on:click={toggleFilter} />
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
                        titleText={$_("guilds.appealtable.selectmembers")}
                        label={$_("guilds.appealtable.selectmembers")}
                        items={members}
                        on:clear={() => onSelect("userIds", [])}
                        on:select={(e) => onSelect("userIds", e.detail.selectedIds)} />
                </div>
                <div>
                    <MultiSelect
                        titleText={$_("guilds.appealtable.statustype")}
                        label={$_("guilds.appealtable.statustype")}
                        items={enums["appealstatus"]}
                        on:clear={() => onSelect("status", [])}
                        on:select={(e) => onSelect("status", e.detail.selectedIds)} />
                </div>
                <div class="self-end">
                    <Button icon={Search32} on:click={executeSearch}>{$_("guilds.appealtable.executesearch")}</Button>
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
            <div class="text-lg font-bold">{$_("guilds.appealtable.nomatches")}</div>
            <div class="text-md">{$_("guilds.appealtable.nomatchesdescription")}</div>
        </div>
    {:else}
        <div class="grid gap-1 grid-cols-1">
            {#each appeals as appeal (appeal.id)}
                <a href={$url(`/guilds/${appeal.guildId}/appeals/${appeal.id}`)}>
                    <Tile class="cursor-pointer mb-2">
                        <div class="flex flex-row items-center">
                            <div class="grow flex-shrink">
                                <div class="flex flex-col grow flex-shrink">
                                    <div class="flex flex-row">
                                        <UserIcon size={matches ? 48 : 32} class="{matches ? 'self-center' : 'self-start'} mr-3" user={appeal.user} />
                                        <div class="flex flex-col flex-shrink">
                                            <div class="flex flex-row items-center">
                                                <AppealStatusTag class="grow-0 shrink-0 !ml-0 mr-1" {appeal} />
                                                <div class="font-bold" style="word-wrap: anywhere">
                                                    {appeal.user?.username ?? appeal.username}#{appeal.user?.discriminator ?? appeal.discriminator}
                                                </div>
                                            </div>
                                            <div>
                                                {appeal.createdAt.format($currentLanguage?.momentDateTimeFormat ?? "MMMM Do YYYY, h:mm:ss")}
                                            </div>
                                        </div>
                                    </div>
                                    <div class="mt-2">
                                        <div style="word-wrap: anywhere">
                                            {$_("guilds.appealtable.hasanswered", { values: { count: appeal.answers.length } })}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <Link href={`/guilds/${appeal.guildId}/appeals/${appeal.id}`} icon={Launch20} class="align-end" />
                        </div>
                    </Tile>
                </a>
            {/each}
        </div>
    {/if}
</MediaQuery>
