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
                name: $_("enums.punishmenttype.warn"),
                data: localWarns,
            },
            {
                name: $_("enums.punishmenttype.mute"),
                data: localMutes,
            },
            {
                name: $_("enums.punishmenttype.kick"),
                data: localKicks,
            },
            {
                name: $_("enums.punishmenttype.ban"),
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


                // fill in missing months with 0 if there are less than 12 months
                for (let i = response.length; i < 12; i++) {
                    localCategories.unshift(moment().subtract(i + 1, "months"));
                    localWarns.unshift(0);
                    localMutes.unshift(0);
                    localKicks.unshift(0);
                    localBans.unshift(0);
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
    title={$_("widgets.guildmodcasecountchart.title")}
    mode={dashboardItem.mode}
    state={$guildDashboardEnableDragging ? WidgetState.Loading : widgetState}>
    <div style="visibility: hidden" use:chart={options} />
    <div slot="loading">
        <div class="dash-widget-list-border flex items-center" style="height: 2rem">
            <SkeletonText />
        </div>
    </div>
    <div slot="empty">
        {$_("widgets.guildmodcasecountchart.empty")}
    </div>
</DashboardWidget>
