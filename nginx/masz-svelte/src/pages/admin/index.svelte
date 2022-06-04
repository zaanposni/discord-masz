<script lang="ts">
    import DashboardConfig from "./../../components/admin/dashboard/DashboardConfig.svelte";
    import { _ } from "svelte-i18n";
    import { WidgetMode } from "../../core/dashboard/WidgetMode";
    import {
        adminDashboardItems,
        adminDashboardToggledItems,
        visibleAdminDashboardItems,
        adminDashboardEnableDragging,
    } from "../../stores/adminDashboardItems";
    import { flip } from "svelte/animate";
    import { dndzone } from "svelte-dnd-action";
    import type { IDashboardItem } from "../../models/IDashboardItem";
    import VersionDisplay from "../../components/admin/dashboard/VersionDisplay.svelte";
    import Monitoring from "../../components/admin/dashboard/Monitoring.svelte";
    import CurrentlyLoggedIn from "../../components/admin/dashboard/CurrentlyLoggedIn.svelte";
    import ResourceStats from "../../components/admin/dashboard/ResourceStats.svelte";
    import CacheVisualization from "../../components/admin/dashboard/CacheVisualization.svelte";
    import Feedback from "../../components/api/Feedback.svelte";

    adminDashboardItems.set([
        {
            id: "admin-version",
            translationKey: "adminversion",
            component: VersionDisplay,
            mode: WidgetMode.x2_1,
        },
        {
            id: "admin-status",
            translationKey: "adminstatus",
            component: Monitoring,
            mode: WidgetMode.x1_1,
        },
        {
            id: "admin-loggedin",
            translationKey: "adminloggedin",
            component: CurrentlyLoggedIn,
            mode: WidgetMode.x1_1,
        },
        {
            id: "admin-resources",
            translationKey: "adminresources",
            component: ResourceStats,
            mode: WidgetMode.x1_1,
        },
        {
            id: "admin-cache",
            translationKey: "admincache",
            component: CacheVisualization,
            mode: WidgetMode.x1_1,
        },
        {
            id: "admin-dashboard-config",
            translationKey: "dashboardconfig",
            component: DashboardConfig,
            mode: WidgetMode.x1_1,
            fix: true,
        },
    ]);

    let localItems: IDashboardItem[];
    const flipDurationMs = 300;
    function handleDndConsider(e) {
        localItems = e.detail.items;
    }

    function handleDndFinalize(e) {
        localItems = e.detail.items;
        adminDashboardToggledItems.set(
            e.detail.items.map((item, index) => {
                return {
                    id: item.id,
                    enabled: true,
                    sortOrder: index,
                };
            })
        );
    }

    function receiveRemoteUpdates(data) {
        if (!$adminDashboardEnableDragging) {
            localItems = data;
        }
    }
    $: receiveRemoteUpdates($visibleAdminDashboardItems);
</script>

<section
    class="grid gap-1 grid-cols-1 md:grid-cols-2 lg:grid-cols-4 xl:grid-cols-6 3xl:grid-cols-12"
    use:dndzone={{ items: localItems, flipDurationMs, dropTargetStyle: {}, morphDisabled: true, dragDisabled: !$adminDashboardEnableDragging }}
    on:consider={handleDndConsider}
    on:finalize={handleDndFinalize}>
    {#if $adminDashboardEnableDragging}
        {#each localItems as item (item.id)}
            <div animate:flip={{ duration: flipDurationMs }} class={item.mode}>
                <svelte:component this={item.component} dashboardItem={item} />
            </div>
        {/each}
    {:else}
        {#each localItems as item (item.id)}
            <svelte:component this={item.component} dashboardItem={item} />
        {/each}
    {/if}
</section>

<Feedback />
