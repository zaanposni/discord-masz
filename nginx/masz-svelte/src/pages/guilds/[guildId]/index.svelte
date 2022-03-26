<script lang="ts">
    import { currentParams } from "./../../../stores/currentParams";
    import { flip } from "svelte/animate";
    import { _ } from "svelte-i18n";
    import type { IAuthUser } from "../../../models/IAuthUser";
    import type { IRouteParams } from "../../../models/IRouteParams";
    import { goto } from "@roxi/routify";
    import { authUser } from "../../../stores/auth";
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { dndzone } from "svelte-dnd-action";
    import { WidgetMode } from "../../../core/dashboard/WidgetMode";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import { toastError } from "../../../services/toast/store";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import LatestModcases from "../../../components/guilds/dashboard/LatestModcases.svelte";

    let dragDisabled = true;
    let items: IDashboardItem[] = [
        {
            id: 1,
            title: "Latest Cases",
            description: "This widget shows the latest cases",
            component: LatestModcases,
            mode: WidgetMode.x2_1,
        },
    ];
    const flipDurationMs = 300;

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
</script>

<section
    use:dndzone={{ items, flipDurationMs, dragDisabled, dropTargetStyle: {} }}
    on:consider={handleDndConsider}
    on:finalize={handleDndFinalize}
    class="grid gap-1 grid-cols-1 md:grid-cols-2 lg:grid-cols-4 xl:grid-cols-6 3xl:grid-cols-12 guilds-list">
    {#each items as item (item.id)}
        <div animate:flip={{ duration: 300 }} class={item.mode === WidgetMode.x1_1 ? "col-span-1" : "col-span-1 md:col-span-2"}>
            <svelte:component this={item.component} dashboardItem={item} />
        </div>
    {/each}
</section>
<div on:click={() => { dragDisabled = !dragDisabled }}>
    Edit this dashboard.
</div>
