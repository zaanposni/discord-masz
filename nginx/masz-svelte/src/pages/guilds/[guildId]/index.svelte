<script lang="ts">
    import LatestModcases from "./../../../components/guilds/dashboard/LatestModcases.svelte";
    import { currentParams } from "./../../../stores/currentParams";
    import { _ } from "svelte-i18n";
    import type { IAuthUser } from "../../../models/IAuthUser";
    import type { IRouteParams } from "../../../models/IRouteParams";
    import { goto } from "@roxi/routify";
    import { authUser } from "../../../stores/auth";
    import { WidgetMode } from "../../../core/dashboard/WidgetMode";
    import {
        guildDashboardItems,
        guildDashboardToggledItems,
        visibleGuildDashboardItems,
        guildDashboardEnableDragging,
    } from "../../../stores/dashboardItems";
    import DashboardConfig from "../../../components/guilds/dashboard/DashboardConfig.svelte";
    import { flip } from "svelte/animate";
    import { dndzone } from "svelte-dnd-action";
    import type { IDashboardItem } from "../../../models/IDashboardItem";

    guildDashboardItems.set([
        {
            id: "latest-modcases",
            translationKey: "latestcases",
            component: LatestModcases,
            mode: WidgetMode.x2_1,
        },
        {
            id: "dashboard-config",
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
        guildDashboardToggledItems.set(
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
        if (!$guildDashboardEnableDragging) {
            localItems = data;
        }
    }

    function checkForModOrHigher(user: IAuthUser, params: IRouteParams) {
        if (user && params?.guildId) {
            if (!user.adminGuilds.map((x) => x.id).includes(params.guildId) && !user.modGuilds.map((x) => x.id).includes(params.guildId)) {
                $goto("/guilds/" + $currentParams.guildId + "/cases");
            }
        }
    }

    $: checkForModOrHigher($authUser, $currentParams);
    $: receiveRemoteUpdates($visibleGuildDashboardItems);
</script>

<section
    class="grid gap-1 grid-cols-1 md:grid-cols-2 lg:grid-cols-4 xl:grid-cols-6 3xl:grid-cols-12"
    use:dndzone={{ items: localItems, flipDurationMs, dropTargetStyle: {}, morphDisabled: true, dragDisabled: !$guildDashboardEnableDragging }}
    on:consider={handleDndConsider}
    on:finalize={handleDndFinalize}>
    {#if $guildDashboardEnableDragging}
        {#each localItems as item (item.id)}
            <div animate:flip={{ duration: flipDurationMs }} class={item.mode === WidgetMode.x1_1 ? "col-span-1" : "col-span-1 md:col-span-2"}>
                <svelte:component this={item.component} dashboardItem={item} />
            </div>
        {/each}
    {:else}
        {#each localItems as item (item.id)}
            <svelte:component this={item.component} dashboardItem={item} />
        {/each}
    {/if}
</section>
