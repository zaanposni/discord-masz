<script lang="ts">
    import { WidgetMode } from "./../../../core/dashboard/WidgetMode";
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";
    import API from "../../../services/api/api";
    import { currentParams } from "../../../stores/currentParams";
    import { CacheMode } from "../../../services/api/CacheMode";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Normal;

    $: $currentParams?.guildId ? loadData() : null;
    function loadData() {
        API.post(`/guilds/${$currentParams.guildId}/modcasetable`, {}, CacheMode.PREFER_CACHE, true)
            .then((response) => {
                console.log(response);
            })
            .catch((error) => {
                console.error(error);
            });
    }
</script>

<DashboardWidget
    title={$_("widgets.latestcases.title")}
    mode={dashboardItem.mode === WidgetMode.x1_1 ? WidgetMode.x1_1 : WidgetMode.x2_1}
    state={widgetState}>
    <div on:click={() => {loadData()}}>hihihi</div>
</DashboardWidget>
