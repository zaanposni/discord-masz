<script lang="ts">
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";

    export let dashboardItem: IDashboardItem;
</script>

<DashboardWidget
    title={$_("widgets.admindiscordinvite.title")}
    mode={dashboardItem.mode}
    state={WidgetState.Normal}>
    <div class="dash-widget-list-border flex flex-row items-center py-2">
        <div class="grow">{$_("widgets.admindiscordinvite.message")}</div>
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2">
        <a href="https://discord.gg/5zjpzw6h3S" target="_blank">
            https://discord.gg/5zjpzw6h3S
        </a>
    </div>
</DashboardWidget>
