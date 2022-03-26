<script lang="ts">
    import { currentParams } from "./../../../stores/currentParams";
    import { flip } from "svelte/animate";
    import { _ } from "svelte-i18n";
    import type { IAuthUser } from "../../../models/IAuthUser";
    import type { IRouteParams } from "../../../models/IRouteParams";
    import { goto } from "@roxi/routify";
    import { authUser } from "../../../stores/auth";
    import { dndzone } from "svelte-dnd-action";
    import { WidgetMode } from "../../../core/dashboard/WidgetMode";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import LatestModcases from "../../../components/guilds/dashboard/LatestModcases.svelte";
    import { guildDashboardItems, visibleGuildDashboardItems } from "../../../stores/dashboardItems";
    import DashboardConfig from "../../../components/guilds/dashboard/DashboardConfig.svelte";

    let dragDisabled = true;
    let items: IDashboardItem[] = [];
    const flipDurationMs = 300;

    guildDashboardItems.set([
        {
            id: "latest-modcases",
            translationKey: "latestmodcases",
            component: LatestModcases,
            mode: WidgetMode.x2_1,
        },
        {
            id: "dashboard-config",
            translationKey: "dashboard-config",
            component: DashboardConfig,
            mode: WidgetMode.x1_1,
            fix: true
        }
    ]);

    function handleDndConsider(e) {
        items = e.detail.items;
    }
    function handleDndFinalize(e) {
        items = e.detail.items;
    }

    function checkForModOrHigher(user: IAuthUser, params: IRouteParams) {
        if (user && params?.guildId) {
            if (!user.adminGuilds.map((x) => x.id).includes(params.guildId) && !user.modGuilds.map((x) => x.id).includes(params.guildId)) {
                $goto("/guilds/" + $currentParams.guildId + "/cases");
            }
        }
    }

    $: checkForModOrHigher($authUser, $currentParams);
    $: items = $visibleGuildDashboardItems;
</script>

<section
    use:dndzone={{ items, flipDurationMs, dragDisabled, dropTargetStyle: {} }}
    on:consider={handleDndConsider}
    on:finalize={handleDndFinalize}
    class="grid gap-1 grid-cols-1 md:grid-cols-2 lg:grid-cols-4 xl:grid-cols-6 3xl:grid-cols-12">
    {#each items as item (item.id)}
        <svelte:component this={item.component} dashboardItem={item} />
    {/each}
</section>
<div
    on:click={() => {
        dragDisabled = !dragDisabled;
    }}>
    Edit this dashboard.
</div>
