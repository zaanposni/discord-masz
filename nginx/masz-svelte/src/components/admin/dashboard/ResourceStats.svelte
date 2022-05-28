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
    title={$_("widgets.adminresources.title")}
    mode={dashboardItem.mode}
    state={$adminDashboardEnableDragging ? WidgetState.Loading : widgetState}>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.adminresources.guilds")}
        </div>
        <div class="mr-1">
            {$adminStats.guilds}
        </div>
        <CircleSolid16 style="fill: var(--cds-ui-04)" />
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.adminresources.cases")}
        </div>
        <div class="mr-1">
            {$adminStats.modCases}
        </div>
        <CircleSolid16 style="fill: var(--cds-ui-04)" />
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.adminresources.automods")}
        </div>
        <div class="mr-1">
            {$adminStats.automodEvents}
        </div>
        <CircleSolid16 style="fill: var(--cds-ui-04)" />
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.adminresources.usernotes")}
        </div>
        <div class="mr-1">
            {$adminStats.userNotes}
        </div>
        <CircleSolid16 style="fill: var(--cds-ui-04)" />
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.adminresources.usermaps")}
        </div>
        <div class="mr-1">
            {$adminStats.userMappings}
        </div>
        <CircleSolid16 style="fill: var(--cds-ui-04)" />
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.adminresources.invites")}
        </div>
        <div class="mr-1">
            {$adminStats.trackedInvites}
        </div>
        <CircleSolid16 style="fill: var(--cds-ui-04)" />
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
