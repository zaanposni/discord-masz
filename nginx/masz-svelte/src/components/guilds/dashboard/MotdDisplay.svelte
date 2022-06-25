<script lang="ts">
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";
    import { SkeletonText } from "carbon-components-svelte";
    import { currentParams } from "../../../stores/currentParams";
    import { guildDashboardEnableDragging } from "../../../stores/dashboardItems";
    import { CacheMode } from "../../../services/api/CacheMode";
    import API from "../../../services/api/api";
    import type { IMotdView } from "../../../models/api/IMotdView";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Loading;

    $: $currentParams?.guildId && !$guildDashboardEnableDragging ? loadData() : null;
    let motd: IMotdView = null;
    function loadData() {
        widgetState = WidgetState.Loading;
        API.get(`/guilds/${$currentParams.guildId}/motd`, CacheMode.PREFER_CACHE, true)
            .then((response: IMotdView) => {
                motd = response;
                widgetState = response?.motd?.showMotd ? WidgetState.Normal : WidgetState.Empty;
            })
            .catch((err) => {
                widgetState = err?.response?.status === 404 ? WidgetState.Empty : WidgetState.Error;
            });
    }
</script>

<DashboardWidget
    title={$_("widgets.guildmotd.title")}
    mode={dashboardItem.mode}
    state={$guildDashboardEnableDragging ? WidgetState.Loading : widgetState}>
    <div class="whitespace-pre-wrap py-2" style="word-wrap: break-word;">
        {motd?.motd?.message}
    </div>
    <div slot="loading">
        <SkeletonText paragraph rows={3} />
    </div>
    <div slot="empty">
        {$_("widgets.guildmotd.empty")}
    </div>
</DashboardWidget>
