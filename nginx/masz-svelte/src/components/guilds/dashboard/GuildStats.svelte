<script lang="ts">
    import { WidgetMode } from "./../../../core/dashboard/WidgetMode";
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";
    import { SkeletonText } from "carbon-components-svelte";
    import { currentParams } from "../../../stores/currentParams";
    import { guildDashboardEnableDragging } from "../../../stores/dashboardItems";
    import { CacheMode } from "../../../services/api/CacheMode";
    import API from "../../../services/api/api";
    import type { IGuildStats } from "../../../models/api/IGuildStats";
    import { CheckmarkFilled16, CircleSolid16 } from "carbon-icons-svelte";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Normal;

    $: $currentParams?.guildId && !$guildDashboardEnableDragging ? loadData() : null;
    let stats: IGuildStats = null;
    function loadData() {
        widgetState = WidgetState.Loading;
        API.get(`/guilds/${$currentParams.guildId}/dashboard/stats`, CacheMode.PREFER_CACHE, true)
            .then((response: IGuildStats) => {
                stats = response;
                widgetState = response ? WidgetState.Normal : WidgetState.Empty;
            })
            .catch(() => {
                widgetState = WidgetState.Error;
            });
    }
</script>

<DashboardWidget
    title={$_("widgets.guildstats.title")}
    mode={dashboardItem.mode}
    state={$guildDashboardEnableDragging ? WidgetState.Loading : widgetState}>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.guildstats.automods")}
        </div>
        <div class="flex flex-row items-center" title={`${stats?.moderationCount} ${$_('widgets.guildstats.automods')}`}>
            <div class="mr-1">
                {stats?.moderationCount}
            </div>
            <CircleSolid16 style="fill: var(--cds-ui-04)" />
        </div>
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.guildstats.invites")}
        </div>
        <div class="flex flex-row items-center" title={`${stats?.trackedInvites} ${$_('widgets.guildstats.invites')}`}>
            <div class="mr-1">
                {stats?.trackedInvites}
            </div>
            <CircleSolid16 style="fill: var(--cds-ui-04)" />
        </div>
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.guildstats.usernotes")}
        </div>
        <div class="flex flex-row items-center" title={`${stats?.userNotes} ${$_('widgets.guildstats.usernotes')}`}>
            <div class="mr-1">
                {stats?.userNotes}
            </div>
            <CircleSolid16 style="fill: var(--cds-ui-04)" />
        </div>
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.guildstats.usermaps")}
        </div>
        <div class="flex flex-row items-center" title={`${stats?.userMappings} ${$_('widgets.guildstats.usermaps')}`}>
            <div class="mr-1">
                {stats?.userMappings}
            </div>
            <CircleSolid16 style="fill: var(--cds-ui-04)" />
        </div>
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.guildstats.banappeals")}
        </div>
        <div class="flex flex-row items-center"  title={`${stats?.appeals} ${$_('widgets.guildstats.banappeals')}`}>
            <div class="mr-1">
                {stats?.appeals}
            </div>
            <CircleSolid16 style="fill: var(--cds-ui-04)" />
        </div>
    </div>
    <div slot="loading">
        <div class="dash-widget-list-border flex items-center" style="height: 2rem">
            <SkeletonText />
        </div>
        <div class="dash-widget-list-border flex items-center" style="height: 2rem">
            <SkeletonText />
        </div>
        <div class="dash-widget-list-border flex items-center" style="height: 2rem">
            <SkeletonText />
        </div>
        <div class="dash-widget-list-border flex items-center" style="height: 2rem">
            <SkeletonText />
        </div>
        <div class="dash-widget-list-border flex items-center" style="height: 2rem">
            <SkeletonText />
        </div>
    </div>
    <div slot="empty">
        {$_("widgets.guildstats.empty")}
    </div>
</DashboardWidget>
