<script lang="ts">
    import { WidgetMode } from "./../../../core/dashboard/WidgetMode";
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";
    import { guildDashboardItems, visibleGuildDashboardItems } from "../../../stores/dashboardItems";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Normal;
</script>

<DashboardWidget
    title={$_("widgets.dashboardconfig.title")}
    mode={dashboardItem.mode === WidgetMode.x1_1 ? WidgetMode.x1_1 : WidgetMode.x2_1}
    state={widgetState}>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">Available</div>
        <div class="self-end">
            {$guildDashboardItems.length}
        </div>
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">Enabled</div>
        <div class="self-end">
            {$visibleGuildDashboardItems.length}
        </div>
    </div>
</DashboardWidget>
