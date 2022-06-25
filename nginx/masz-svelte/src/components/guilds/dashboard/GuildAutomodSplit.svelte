<script lang="ts">
    import { WidgetMode } from "./../../../core/dashboard/WidgetMode";
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";
    import { SkeletonText } from "carbon-components-svelte";
    import { currentParams } from "../../../stores/currentParams";
    import { guildDashboardEnableDragging } from "../../../stores/dashboardItems";
    import { CacheMode } from "../../../services/api/CacheMode";
    import API from "../../../services/api/api";
    import { isDarkMode } from "../../../stores/theme";
    import { chart } from "../../../core/charts/chart.js";
    import type { IAutomodSplitEntry } from "../../../models/api/IAutomodSplitEntry";
    import { AutomodType } from "../../../models/api/AutomodType";
    import { currentLanguage } from "../../../stores/currentLanguage";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Normal;

    let options;
    $: options = {
        chart: {
            type: "donut",
            height: "100%",
            width: "100%",
            toolbar: {
                show: false,
            },
            foreColor: "var(--cds-text-01)",
            background: "var(--cds-ui-01)",
        },
        plotOptions: {
            pie: {
                donut: {
                    labels: {
                        show: true,
                        total: {
                            show: true,
                            showAlways: true,
                        },
                    },
                },
            },
        },
        theme: {
            mode: $isDarkMode ? "dark" : "light",
            monochrome: {
                enabled: true,
            },
        },
        series: localData.map((entry) => entry.count),
        labels: localData.map((entry) => getAutomodNameByTypeId(entry.type)),
        dataLabels: {
            enabled: false,
        },
        legend: {
            show: false,
        },
    };

    function getAutomodNameByTypeId(id: AutomodType): string {
        return $_(`enums.automoderationtype.${AutomodType[id].toLowerCase()}`);
    }

    let localData: IAutomodSplitEntry[] = [];
    $: $currentParams?.guildId && !$guildDashboardEnableDragging ? loadData() : null;
    function loadData() {
        widgetState = WidgetState.Loading;
        API.get(`/guilds/${$currentParams.guildId}/dashboard/automodchart`, CacheMode.PREFER_CACHE, true)
            .then((response: IAutomodSplitEntry[]) => {
                localData = response;
                widgetState = response.length ? WidgetState.Normal : WidgetState.Empty;
            })
            .catch(() => {
                widgetState = WidgetState.Error;
            });
    }
</script>

<DashboardWidget
    lowPaddingMode
    title={$_("widgets.guildautomodsplit.title")}
    mode={dashboardItem.mode}
    state={$guildDashboardEnableDragging ? WidgetState.Loading : widgetState}>
    <div style="" use:chart={options} />
    <div slot="loading">
        <div class="dash-widget-list-border flex items-center" style="height: 2rem">
            <SkeletonText />
        </div>
    </div>
    <div slot="empty">
        {$_("widgets.guildautomodsplit.empty")}
    </div>
</DashboardWidget>
