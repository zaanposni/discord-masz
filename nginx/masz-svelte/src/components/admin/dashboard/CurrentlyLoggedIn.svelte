<script lang="ts">
    import { adminDashboardEnableDragging } from "./../../../stores/adminDashboardItems";
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";
    import { writable } from "svelte/store";
    import type { Writable } from "svelte/store";
    import { SkeletonText } from "carbon-components-svelte";
    import type { IAdminStats } from "../../../models/api/IAdminStats";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";

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
</script>

<DashboardWidget
    title={$_("widgets.adminloggedin.title")}
    mode={dashboardItem.mode}
    state={$adminDashboardEnableDragging ? WidgetState.Loading : widgetState}>
    {#each $adminStats.loginsInLast15Minutes as login}
        <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
            <div class="grow">
                {login}
            </div>
        </div>
    {/each}
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
