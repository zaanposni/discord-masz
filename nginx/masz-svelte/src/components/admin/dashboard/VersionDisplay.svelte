<script lang="ts">
	import { adminDashboardEnableDragging } from './../../../stores/adminDashboardItems';
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Normal;
</script>

<DashboardWidget
    title={$_("widgets.adminversion.title")}
    mode={dashboardItem.mode}
    state={$adminDashboardEnableDragging ? WidgetState.Loading : widgetState}>
    i bims version
    <div slot="loading">
        loading
    </div>
</DashboardWidget>
