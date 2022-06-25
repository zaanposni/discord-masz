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
    title={$_("widgets.guildpunishmentstats.title")}
    mode={dashboardItem.mode}
    state={$guildDashboardEnableDragging ? WidgetState.Loading : widgetState}>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("warns")}
        </div>
        <div class="flex flex-row items-center">
            <div class="mr-1" title={`${stats?.warnCount} ${$_("warns")}`}>
                {stats?.warnCount}
            </div>
            <CircleSolid16 style="fill: var(--cds-ui-04)" title={`${stats?.warnCount} ${$_("warns")}`} />
        </div>
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("mutes")}
        </div>
        <div class="flex flex-row items-center">
            {#if stats?.activeMuteCount}
                <div class="mr-1" title={`${stats?.activeMuteCount} ${$_("widgets.guildpunishmentstats.activemutes")}`}>
                    {stats?.activeMuteCount}
                </div>
                <CheckmarkFilled16
                    class="mr-3"
                    style="fill: var(--cds-support-success)"
                    title={`${stats?.activeMuteCount} ${$_("widgets.guildpunishmentstats.activemutes")}`} />
            {/if}
            <div class="mr-1" title={`${stats?.muteCount} ${$_("mutes")}`}>
                {stats?.muteCount}
            </div>
            <CircleSolid16 style="fill: var(--cds-ui-04)" title={`${stats?.muteCount} ${$_("mutes")}`} />
        </div>
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("kicks")}
        </div>
        <div class="flex flex-row items-center">
            <div class="mr-1" title={`${stats?.kickCount} ${$_("kicks")}`}>
                {stats?.kickCount}
            </div>
            <CircleSolid16 style="fill: var(--cds-ui-04)" title={`${stats?.kickCount} ${$_("kicks")}`} />
        </div>
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("bans")}
        </div>
        <div class="flex flex-row items-center">
            {#if stats?.activeBanCount}
                <div class="mr-1" title={`${stats?.activeBanCount} ${$_("widgets.guildpunishmentstats.activebans")}`}>
                    {stats?.activeBanCount}
                </div>
                <CheckmarkFilled16
                    class="mr-3"
                    style="fill: var(--cds-support-success)"
                    title={`${stats?.activeBanCount} ${$_("widgets.guildpunishmentstats.activebans")}`} />
            {/if}
            <div class="mr-1" title={`${stats?.banCount} ${$_("bans")}`}>
                {stats?.banCount}
            </div>
            <CircleSolid16 style="fill: var(--cds-ui-04)" title={`${stats?.banCount} ${$_("bans")}`} />
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
    </div>
    <div slot="empty">
        {$_("widgets.guildpunishmentstats.empty")}
    </div>
</DashboardWidget>
