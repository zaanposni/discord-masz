<script lang="ts">
    import type { IModeratorCasesCount } from "./../../../models/api/IModeratorCasesCount";
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";
    import { SkeletonText } from "carbon-components-svelte";
    import { currentParams } from "../../../stores/currentParams";
    import { guildDashboardEnableDragging } from "../../../stores/dashboardItems";
    import { isDarkMode } from "../../../stores/theme";
    import { chart } from "../../../core/charts/chart.js";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Normal;

    let localData: any[] = [];
    let options;
    $: options = {
        chart: {
            type: "bar",
            height: "100%",
            width: "100%",
            toolbar: {
                show: false,
            },
            foreColor: "var(--cds-text-01)",
            background: "var(--cds-ui-01)",
        },
        grid: {
            show: false,
        },
        plotOptions: {
            bar: {
                horizontal: true,
            },
        },
        theme: {
            mode: $isDarkMode ? "dark" : "light",
            monochrome: {
                enabled: true,
            },
        },
        series: [
            {
                name: $_("widgets.guildmoderatorcases.cases"),
                data: localData,
            },
        ],
        dataLabels: {
            enabled: false,
        },
        legend: {
            show: false,
        },
        yaxis: {
            labels: {
                maxWidth: 100,
            },
        },
    };

    $: $currentParams?.guildId && !$guildDashboardEnableDragging ? loadData() : null;
    function loadData() {
        widgetState = WidgetState.Loading;
        API.get(`/guilds/${$currentParams.guildId}/dashboard/moderatorcases`, CacheMode.PREFER_CACHE, true)
            .then((response: IModeratorCasesCount[]) => {
                localData = response.map((data) => {
                    return {
                        x: data.modName,
                        y: data.count,
                    };
                }).slice(0, 12);

                // fill in array if there are less than 8 entries
                for (let i = response.length; i < 8; i++) {
                    localData.push({
                        x: "",
                        y: 0,
                    });
                }

                widgetState = response.length ? WidgetState.Normal : WidgetState.Empty;
            })
            .catch(() => {
                widgetState = WidgetState.Error;
            });
    }
</script>

<DashboardWidget
    lowPaddingMode
    title={$_("widgets.guildmoderatorcases.title")}
    mode={dashboardItem.mode}
    state={$guildDashboardEnableDragging ? WidgetState.Loading : widgetState}>
    <div style="visibility: hidden" use:chart={options} />
    <div slot="loading">
        <div class="dash-widget-list-border flex items-center" style="height: 2rem">
            <SkeletonText />
        </div>
    </div>
    <div slot="empty">
        {$_("widgets.guildmoderatorcases.empty")}
    </div>
</DashboardWidget>
