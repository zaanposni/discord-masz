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

    let dragDisabled = false;
    let items = [
        { id: 1, name: "item1" },
        { id: 2, name: "item2" },
        { id: 3, name: "item3" },
        { id: 4, name: "item4" },
        { id: 5, name: "item4" },
        { id: 6, name: "item4" },
        { id: 7, name: "item4" },
        { id: 8, name: "item4" },
        { id: 9, name: "item4" },
        { id: 10, name: "item4" },
        { id: 11, name: "item4" },
        { id: 12, name: "item4" },
        { id: 13, name: "item4" },
        { id: 14, name: "item4" },
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
        <div animate:flip="{{duration: 300}}" class={item.id % 4 ? "col-span-1" : "col-span-1 md:col-span-2"}>
            <DashboardWidget mode={item.id % 4 ? WidgetMode.x1_1 : WidgetMode.x2_1}>hi</DashboardWidget>
        </div>
    {/each}
</section>
