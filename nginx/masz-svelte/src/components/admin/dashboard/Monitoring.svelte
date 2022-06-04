<script lang="ts">
    import { adminDashboardEnableDragging } from "./../../../stores/adminDashboardItems";
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";
    import { writable } from "svelte/store";
    import type { Writable } from "svelte/store";
    import { CircleSolid16 } from "carbon-icons-svelte";
    import { SkeletonText } from "carbon-components-svelte";
    import type { IAdminStats } from "../../../models/api/IAdminStats";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import type { IStatusDetail } from "../../../models/api/IStatusDetail";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Loading;

    const adminStats: Writable<IAdminStats> = writable(null);

    API.get("/meta/adminstats", CacheMode.API_ONLY, false)
        .then((res) => {
            adminStats.set(res);
            widgetState = WidgetState.Normal;
        })
        .catch(() => {
            widgetState = WidgetState.Error;
        });

    function calcColor(status: IStatusDetail, warningLimit: number, errorLimit: number) {
        if (!status.online) {
            return "--cds-ui-04";
        }
        if (status.responseTime < warningLimit) {
            return "--cds-support-success-inverse";
        } else if (status.responseTime < errorLimit) {
            return "--cds-support-warning-inverse";
        } else {
            return "--cds-support-error-inverse";
        }
    }
</script>

<DashboardWidget
    title={$_("widgets.adminstatus.title")}
    mode={dashboardItem.mode}
    state={$adminDashboardEnableDragging ? WidgetState.Loading : widgetState}>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.adminstatus.bot")}
        </div>
        <div class="mr-1">
            {#if $adminStats.botStatus.online}
                {$adminStats.botStatus.responseTime.toFixed(0)}ms
            {:else if $adminStats.botStatus.message}
                {$adminStats.botStatus.message}
            {:else}
                {$_("widgets.adminstatus.offline")}
            {/if}
        </div>
        <CircleSolid16 style="fill: var({calcColor($adminStats.botStatus, 200, 400)})" />
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.adminstatus.db")}
        </div>
        <div class="mr-1">
            {#if $adminStats.dbStatus.online}
                {$adminStats.dbStatus.responseTime.toFixed(0)}ms
            {:else if $adminStats.dbStatus.message}
                {$adminStats.dbStatus.message}
            {:else}
                {$_("widgets.adminstatus.offline")}
            {/if}
        </div>
        <CircleSolid16 style="fill: var({calcColor($adminStats.dbStatus, 30, 60)})" />
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.adminstatus.cache")}
        </div>
        <div class="mr-1">
            {#if $adminStats.cacheStatus.online}
                {$adminStats.cacheStatus.responseTime.toFixed(0)}ms
            {:else if $adminStats.cacheStatus.message}
                {$adminStats.cacheStatus.message}
            {:else}
                {$_("widgets.adminstatus.offline")}
            {/if}
        </div>
        <CircleSolid16 style="fill: var({calcColor($adminStats.cacheStatus, 1, 5)})" />
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
    </div>
</DashboardWidget>
