<script lang="ts">
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
    } from "carbon-icons-svelte";
    import { Link, SkeletonPlaceholder, SkeletonText, Tag, Tile } from "carbon-components-svelte";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import { currentParams } from "./../../../stores/currentParams";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import { Pagination } from "carbon-components-svelte";
    import { PaginationSkeleton } from "carbon-components-svelte";
    import { _ } from "svelte-i18n";
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
            events = [];
        }
        lastUsedGuildId = $currentParams?.guildId;
        API.get(`/guilds/${$currentParams.guildId}/automoderations?startPage=${page - 1}`, CacheMode.PREFER_CACHE, true)
            .then((response: { events: IAutomodEntry[]; count: number }) => {
                events = response.events;
                fullSize = response.count;
                loading = false;
                initialLoading = false;
            })
            .catch(() => {
                loading = false;
                initialLoading = false;
                toastError($_("guilds.modcasetable.failedtoload"));
            });
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
                        <div class="grow flex-shink">
                            <div class="flex flex-col grow flex-shink">
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
            <div class="text-lg font-bold">{$_("guilds.modcasetable.nomatches")}</div>
            <div class="text-md">{$_("guilds.modcasetable.nomatchesdescription")}</div>
        </div>
    {:else}
        <div class="grid gap-1 grid-cols-1">
            {#each events as event}
                <Tile class="mb-2">
                    <div class="flex flex-row items-center">
                        <div class="grow flex-shink">
                            <div class="flex flex-col grow flex-shink">
                                <div class="flex flex-row">
                                    <div class="flex flex-col">
                                        <div class="flex flex-row flex-wrap items-center">
                                            <svelte:component this={getIconByAutomodType(event.autoModerationType)} />
                                            <div class="mx-2 font-bold" style="word-wrap: anywhere">
                                                {event.username}#{event.discriminator}
                                            </div>
                                            <div>
                                                {event.createdAt.format($currentLanguage.momentDateTimeFormat)}
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
