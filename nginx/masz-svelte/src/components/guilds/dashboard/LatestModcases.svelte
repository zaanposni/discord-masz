<script lang="ts">
    import type { ICompactCaseView } from "./../../../models/api/ICompactCaseView";
    import { WidgetMode } from "./../../../core/dashboard/WidgetMode";
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";
    import API from "../../../services/api/api";
    import { currentParams } from "../../../stores/currentParams";
    import { CacheMode } from "../../../services/api/CacheMode";
    import { Link, SkeletonText, Truncate } from "carbon-components-svelte";
    import PunishmentTag from "../../api/PunishmentTag.svelte";
    import UserIcon from "../../discord/UserIcon.svelte";
    import { Launch20 } from "carbon-icons-svelte";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import { guildDashboardEnableDragging } from "../../../stores/dashboardItems";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Loading;

    $: $currentParams?.guildId && !$guildDashboardEnableDragging ? loadData() : null;
    let cases: ICompactCaseView[] = [];
    function loadData() {
        widgetState = WidgetState.Loading;
        API.post(`/guilds/${$currentParams.guildId}/modcasetable`, {}, CacheMode.PREFER_CACHE, true)
            .then((response: { cases: ICompactCaseView[] }) => {
                cases = response.cases.slice(0, 5);
                widgetState = response.cases.length ? WidgetState.Normal : WidgetState.Empty;
            })
            .catch(() => {
                widgetState = WidgetState.Error;
            });
    }
</script>

<MediaQuery query="(min-width: 768px)" let:matches>
    <DashboardWidget
        title={$_("widgets.latestcases.title")}
        mode={dashboardItem.mode}
        state={$guildDashboardEnableDragging ? WidgetState.Loading : widgetState}>
        {#each cases as entry}
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
        <div slot="loading">
            <div class="dash-widget-list-border flex items-center" style="height: 2.5rem">
                <SkeletonText />
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
            <div class="dash-widget-list-border flex items-center" style="height: 2.5 rem">
                <SkeletonText />
            </div>
        </div>
        <div slot="empty">
            {$_("widgets.latestcases.empty")}
        </div>
    </DashboardWidget>
</MediaQuery>
