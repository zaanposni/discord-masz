<script lang="ts">
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";
    import API from "../../../services/api/api";
    import { currentParams } from "../../../stores/currentParams";
    import { CacheMode } from "../../../services/api/CacheMode";
    import { InlineLoading, InlineNotification, Link, SkeletonText, Truncate } from "carbon-components-svelte";
    import PunishmentTag from "../../api/PunishmentTag.svelte";
    import UserIcon from "../../discord/UserIcon.svelte";
    import { Launch20 } from "carbon-icons-svelte";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import { guildDashboardEnableDragging } from "../../../stores/dashboardItems";
    import { Search } from "carbon-components-svelte";
    import { toastError } from "../../../services/toast/store";
    import type { IQuickSearchResult } from "../../../models/api/IQuickSearchResult";
    import { clearSearchHistory, putInSearchHistory, searchHistory } from "../../../stores/guildSearchHistory";
    import Robot from "carbon-pictograms-svelte/lib/Robot.svelte";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Normal;

    let searchResults: IQuickSearchResult = {
        cases: [],
        userMappingViews: [],
        userNoteView: null,
    };

    let searchValue = "";
    let debounceTimer;
    let searching: boolean = false;
    let searchDone: boolean = false;

    $: searchValue !== null ? loadData() : undefined;
    function loadData() {
        if (debounceTimer) {
            clearTimeout(debounceTimer);
        }
        if (searchValue.trim().length > 2) {
            searching = true;
            searchDone = false;
            debounceTimer = setTimeout(() => {
                putInSearchHistory($currentParams.guildId, searchValue.trim());
                API.get(`/guilds/${$currentParams.guildId}/dashboard/search?search=${searchValue.trim()}`, CacheMode.API_ONLY, false)
                    .then((res: IQuickSearchResult) => {
                        res.cases = res.cases.slice(0, 8);
                        res.userMappingViews = res.userMappingViews.slice(0, 8);
                        searchDone = true;
                        searchResults = res;
                        searching = false;
                    })
                    .catch((err) => {
                        toastError("Something went wrong.");
                        searching = false;
                        searchDone = true;
                    });
            }, 500);
        } else {
            onClear();
        }
    }

    function onClear() {
        searchResults = {
            cases: [],
            userMappingViews: [],
            userNoteView: null,
        };
        searching = false;
        searchDone = false;
    }

    function clickOnHistoryEntry(entry: string) {
        searching = true;
        searchDone = false;
        searchValue = entry;
    }
</script>

<MediaQuery query="(min-width: 768px)" let:matches>
    <DashboardWidget
        title={$_("widgets.guildquicksearch.title")}
        mode={dashboardItem.mode}
        state={$guildDashboardEnableDragging ? WidgetState.Loading : widgetState}>
        <Search placeholder={$_("widgets.guildquicksearch.placeholder")} bind:value={searchValue} on:clear={onClear} />
        {#if !searching}
            {#if searchResults?.cases?.length != 0 || searchResults?.userMappingViews?.length != 0 || searchResults?.userNoteView != null}
                <!-- TODO: show usernote -->
                <!-- TODO: show usermaps -->

                <div class="flex flex-col">
                    {#each searchResults.cases as entry (entry.modCase.id)}
                        <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2.5rem">
                            <div class="grow shrink-0 mr-1">
                                <UserIcon user={entry.suspect} />
                            </div>
                            {#if matches}
                                <PunishmentTag class="grow shrink-0 mr-1" modCase={entry.modCase} />
                            {/if}
                            <Truncate title={entry.modCase.title}>
                                {entry.modCase.title}
                            </Truncate>
                            <Link href={`/guilds/${entry.modCase.guildId}/cases/${entry.modCase.caseId}`} icon={Launch20} class="align-end" />
                        </div>
                    {/each}
                </div>
            {:else if $searchHistory && !searchValue}
                <div class="flex flex-row my-4 pb-2" style="border-bottom: 1px solid var(--cds-ui-03, #f0f3f6);">
                    <div>{$_("widgets.guildquicksearch.searchhistory")} ({$searchHistory.length})</div>
                    <div class="grow" />
                    {#if $searchHistory.length}
                        <div
                            class="cursor-pointer"
                            on:click={() => {
                                clearSearchHistory($currentParams.guildId);
                            }}>
                            {$_("widgets.guildquicksearch.clearhistory")}
                        </div>
                    {/if}
                </div>
                {#each $searchHistory as entry}
                    <div
                        class="dash-widget-list-border cursor-pointer flex items-center"
                        style="height: 2rem"
                        on:click={() => {
                            clickOnHistoryEntry(entry);
                        }}>
                        <Truncate title={entry}>
                            {entry}
                        </Truncate>
                    </div>
                {/each}
            {:else if searchDone && searchValue}
                <div class="flex flex-row my-4 pb-2" style="border-bottom: 1px solid var(--cds-ui-03, #f0f3f6);">
                    <InlineLoading status="error" description={$_("widgets.guildquicksearch.searchfailed")} />
                </div>
                <InlineNotification
                  title={$_("error")}
                  subtitle={$_("widgets.guildquicksearch.noresults")}
                />
            {/if}
        {:else}
            <div class="flex flex-row my-4 pb-2" style="border-bottom: 1px solid var(--cds-ui-03, #f0f3f6);">
                <InlineLoading description={$_("widgets.guildquicksearch.searching")} />
            </div>
            <div class="dash-widget-list-border flex items-center" style="height: 2.5rem">
                <SkeletonText />
            </div>
            <div class="dash-widget-list-border flex items-center" style="height: 2.5rem">
                <SkeletonText />
            </div>
            <div class="dash-widget-list-border flex items-center" style="height: 2.5rem">
                <SkeletonText />
            </div>
        {/if}
        <div slot="loading">
            <Search skeleton />
            <div class="flex flex-row my-4 pb-2" style="border-bottom: 1px solid var(--cds-ui-03, #f0f3f6);">
                <SkeletonText class="shrink" />
                <div class="w-1/3 grow" />
                <SkeletonText class="shrink" />
            </div>
            <div class="dash-widget-list-border flex items-center py-2" style="height: 2.5rem">
                <SkeletonText />
            </div>
            <div class="dash-widget-list-border flex items-center py-2" style="height: 2.5rem">
                <SkeletonText />
            </div>
            <div class="dash-widget-list-border flex items-center py-2" style="height: 2.5rem">
                <SkeletonText />
            </div>
            <div class="dash-widget-list-border flex items-center py-2" style="height: 2.5rem">
                <SkeletonText />
            </div>
            <div class="dash-widget-list-border flex items-center py-2" style="height: 2.5 rem">
                <SkeletonText />
            </div>
        </div>
    </DashboardWidget>
</MediaQuery>
