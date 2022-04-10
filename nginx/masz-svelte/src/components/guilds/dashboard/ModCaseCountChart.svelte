<script lang="ts">
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
    import type { ICaseCount } from "../../../models/api/ICaseCount";
    import moment from "moment";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Normal;

    let localCategories: moment.Moment[] = [];
    let localWarns: number[] = [];
    let localMutes: number[] = [];
    let localKicks: number[] = [];
    let localBans: number[] = [];

    let options;
    $: options = {
        chart: {
            type: "bar",
            stacked: true,
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
        theme: {
            mode: $isDarkMode ? "dark" : "light",
            monochrome: {
                enabled: false,
            },
        },
        series: [
            {
                name: $_("warns"),
                data: localWarns,
            },
            {
                name: $_("mutes"),
                data: localMutes,
            },
            {
                name: $_("kicks"),
                data: localKicks,
            },
            {
                name: $_("bans"),
                data: localBans,
            },
        ],
        dataLabels: {
            enabled: false,
        },
        legend: {
            show: true,
        },
        xaxis: {
            categories: localCategories,
            labels: {
                formatter: function (value, timestamp) {
                    return moment(timestamp).format("MMM YY");
                },
            },
        },
    };

    $: $currentParams?.guildId && !$guildDashboardEnableDragging ? loadData() : null;
    function loadData() {
        widgetState = WidgetState.Loading;
        API.get(`/guilds/${$currentParams.guildId}/dashboard/casecountchart`, CacheMode.PREFER_CACHE, true)
            .then((response: ICaseCount[]) => {
                localCategories = [];
                localWarns = [];
                localMutes = [];
                localKicks = [];
                localBans = [];

                response.reverse().map((item) => {
                    localWarns.push(item.warnCount);
                    localMutes.push(item.muteCount);
                    localKicks.push(item.kickCount);
                    localBans.push(item.banCount);

                    // create moment from item.year and item.month
                    localCategories.push(moment(`${item.year}-${item.month}`, "YYYY-MM"));
                });

                widgetState = response.length ? WidgetState.Normal : WidgetState.Empty;
            })
            .catch(() => {
                widgetState = WidgetState.Error;
            });
    }
</script>

<DashboardWidget
    lowPaddingMode
    title={$_("widgets.guildmodcasecountchart.title")}
    mode={dashboardItem.mode}
    state={$guildDashboardEnableDragging ? WidgetState.Loading : widgetState}>
    <div style="visibility: hidden" use:chart={options} />
    <div slot="loading">
        <div class="dash-widget-list-border flex items-center" style="height: 2rem">
            <SkeletonText />
        </div>
    </div>
</DashboardWidget>
