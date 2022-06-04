<script lang="ts">
    import type { IDiscordUser } from "./../../../models/discord/IDiscordUser";
    import {
        Launch20,
        Email20,
        FaceSatisfied20,
        Group20,
        DocumentAttachment20,
        ContentView20,
        RecentlyViewed20,
        ErrorOutline20,
        AnalyticsCustom20,
        TextScale20,
        Link20,
        Search32,
        Filter24,
    } from "carbon-icons-svelte";
    import { Button, Link, MultiSelect, SkeletonText, Tag, Tile } from "carbon-components-svelte";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import { currentParams } from "./../../../stores/currentParams";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import { Pagination } from "carbon-components-svelte";
    import { PaginationSkeleton } from "carbon-components-svelte";
    import { _ } from "svelte-i18n";
    import { slide } from "svelte/transition";
    import Warning_02 from "carbon-pictograms-svelte/lib/Warning_02.svelte";
    import { toastError } from "../../../services/toast/store";
    import type { IAutomodEntry } from "../../../models/api/IAutomodEntry";
    import { AutomodType } from "../../../models/api/AutomodType";
    import AutomoderationTypes from "../../../services/enums/AutomoderationType";
    import AutomoderationActions from "../../../services/enums/AutomoderationAction";
    import { currentLanguage } from "../../../stores/currentLanguage";

    let events: IAutomodEntry[] = [];
    let initialLoading: boolean = true;
    let loading: boolean = true;
    let fullSize: number = 0;
    let currentPage: number = 1;

    let members: { id: string; text: string }[] = [];
    let filterOpened: boolean = false;
    let filter: any = {};
    let enums = {};

    $: enums = {
        automodType: AutomoderationTypes.getAll().map((x) => ({
            id: x.id.toString(),
            text: $_(x.translationKey),
        })),
        automodAction: AutomoderationActions.getAll().map((x) => ({
            id: x.id.toString(),
            text: $_(x.translationKey),
        })),
    };

    $: forwardText = $_("core.pagination.forwardtext");
    $: backwardText = $_("core.pagination.backwardtext");
    $: itemRangeText = (min, max, total) => $_("core.pagination.itemrangetext", { values: { min, max, total } });
    $: pageRangeText = (current, total) => $_(`core.pagination.pagerangetext${total === 1 ? "" : "plural"}`, { values: { total } });

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
            events = [];
        }
        lastUsedGuildId = $currentParams?.guildId;
        let filterIsSet = Object.keys(filter).length > 0;
        API.post(
            `/guilds/${$currentParams.guildId}/automoderations?startPage=${page - 1}`,
            filter,
            filterIsSet ? CacheMode.API_ONLY : CacheMode.PREFER_CACHE,
            !filterIsSet
        )
            .then((response: { events: IAutomodEntry[]; count: number }) => {
                events = response.events;
                fullSize = response.count;
                loading = false;
                initialLoading = false;
            })
            .catch(() => {
                loading = false;
                initialLoading = false;
                toastError($_("guilds.automodtable.failedtoload"));
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

    function getIconByAutomodType(type: AutomodType) {
        switch (type) {
            case AutomodType.InvitePosted:
                return Email20;
            case AutomodType.TooManyEmotes:
                return FaceSatisfied20;
            case AutomodType.TooManyMentions:
                return Group20;
            case AutomodType.TooManyAttachments:
                return DocumentAttachment20;
            case AutomodType.TooManyEmbeds:
                return ContentView20;
            case AutomodType.TooManyAutoModerations:
                return RecentlyViewed20;
            case AutomodType.CustomWordFilter:
                return ErrorOutline20;
            case AutomodType.TooManyMessages:
                return AnalyticsCustom20;
            case AutomodType.TooManyDuplicatedCharacters:
                return TextScale20;
            default:
                return Link20;
        }
    }

    function executeSearch() {
        loading = true;
        events = [];
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
    :global(.automodevent-tag-list .bx--tag:not(:first-child)) {
        margin-left: 0.25rem;
    }
    :global(.automodevent-tag-list .bx--tag) {
        margin-left: 0;
    }
</style>

<MediaQuery query="(min-width: 768px)" let:matches>
    <!-- Header -->
    <div class="flex flex-col mb-4">
        <h2 class="font-weight-bold mb-4">
            {$_("nav.guild.automods")}
        </h2>
        <div class="flex flex-row">
            <Button iconDescription={$_("guilds.automodtable.useadvancedfilter")} icon={Filter24} on:click={toggleFilter} />
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
                        titleText={$_("guilds.automodtable.selectmembers")}
                        label={$_("guilds.automodtable.selectmembers")}
                        items={members}
                        on:clear={() => onSelect("userIds", [])}
                        on:select={(e) => onSelect("userIds", e.detail.selectedIds)} />
                </div>
                <div>
                    <MultiSelect
                        titleText={$_("guilds.automodtable.selecttypes")}
                        label={$_("guilds.automodtable.selecttypes")}
                        items={enums["automodType"]}
                        on:clear={() => onSelect("types", [])}
                        on:select={(e) => onSelect("types", e.detail.selectedIds)} />
                </div>
                <div>
                    <MultiSelect
                        titleText={$_("guilds.automodtable.selectactions")}
                        label={$_("guilds.automodtable.selectactions")}
                        items={enums["automodAction"]}
                        on:clear={() => onSelect("actions", [])}
                        on:select={(e) => onSelect("actions", e.detail.selectedIds)} />
                </div>
                <div class="self-end">
                    <Button icon={Search32} on:click={executeSearch}>{$_("guilds.automodtable.executesearch")}</Button>
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
                                <div class="flex flex-row items-center">
                                    <SkeletonText width={"5%"} />
                                    <div class="mr-5" />
                                    <SkeletonText width={"20%"} />
                                </div>
                                <SkeletonText paragraph lines={4} class="mt-2" />
                            </div>
                        </div>
                    </div>
                </Tile>
            {/each}
        </div>
    {:else if fullSize === 0}
        <div class="flex flex-col ml-2" class:items-center={!matches}>
            <Warning_02 />
            <div class="text-lg font-bold">{$_("guilds.automodtable.nomatches")}</div>
            <div class="text-md">{$_("guilds.automodtable.nomatchesdescription")}</div>
        </div>
    {:else}
        <div class="grid gap-1 grid-cols-1">
            {#each events as event}
                <Tile class="mb-2">
                    <div class="flex flex-row items-center">
                        <div class="grow flex-shrink">
                            <div class="flex flex-col grow flex-shrink">
                                <div class="flex flex-row">
                                    <div class="flex flex-col">
                                        <div class="flex flex-row flex-wrap items-center">
                                            <svelte:component this={getIconByAutomodType(event.autoModerationType)} />
                                            <div class="mx-2 font-bold" style="word-wrap: anywhere">
                                                {event.username}#{event.discriminator}
                                            </div>
                                            <div>
                                                {event.createdAt?.format($currentLanguage?.momentDateTimeFormat ?? "MMMM Do YYYY, h:mm:ss")}
                                            </div>
                                        </div>
                                        <div class="flex flex-row flex-wrap items-center automodevent-tag-list mt-2">
                                            <Tag type="outline">
                                                {$_(AutomoderationTypes.getById(event.autoModerationType))}
                                            </Tag>
                                            <Tag type="red">
                                                {$_(AutomoderationActions.getById(event.autoModerationAction))}
                                            </Tag>
                                        </div>
                                    </div>
                                </div>
                                <div class="mt-2">
                                    <span title={event.messageContent} style="word-break: break-all; white-space: pre-wrap;">
                                        {event.messageContent}
                                    </span>
                                </div>
                            </div>
                        </div>
                        {#if event.associatedCaseId}
                            <Link href={`/guilds/${event.guildId}/cases/${event.associatedCaseId}`} icon={Launch20} class="align-end" />
                        {/if}
                    </div>
                </Tile>
            {/each}
        </div>
    {/if}
</MediaQuery>
