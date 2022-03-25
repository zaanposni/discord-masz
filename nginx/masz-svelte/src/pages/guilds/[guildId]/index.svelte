<script lang="ts">
    import { currentParams } from "./../../../stores/currentParams";
    import { _, locale } from "svelte-i18n";
    import type { IAuthUser } from "../../../models/IAuthUser";
    import type { IRouteParams } from "../../../models/IRouteParams";
    import { goto } from "@roxi/routify";
    import { authUser } from "../../../stores/auth";
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetMode } from "../../../core/dashboard/WidgetMode";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import { chart } from "../../../core/charts/chart.js";
    import { isDarkMode } from "../../../stores/theme";

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
        plotOptions: {
            bar: {
                horizontal: true,
            },
        },
        theme: {
            mode: $isDarkMode ? "dark" : "light",
            monochrome: {
                enabled: false,
            },
        },
        grid: {
            show: false,
        },
        series: [
            {
                name: "sales",
                data: [30, 40, 35, 50, 49, 60, 70, 91, 125],
            },
        ],
        dataLabels: {
            enabled: false,
        },
        yaxis: {
            opposite: true,
        },
        xaxis: {
            categories: [1991, 1992, 1993, 1994, 1995, 1996, 1997, 1998, 1999],
            axisBorder: {
                show: false,
            },
            tickPlacement: "on",
        },
    };

    function checkForModOrHigher(user: IAuthUser, params: IRouteParams) {
        if (user && params?.guildId) {
            if (!user.adminGuilds.map((x) => x.id).includes(params.guildId) && !user.modGuilds.map((x) => x.id).includes(params.guildId)) {
                $goto("/guilds/" + $currentParams.guildId + "/cases");
            }
        }
    }
    $: checkForModOrHigher($authUser, $currentParams);
</script>

<div class="grid gap-4 grid-cols-1 md:grid-cols-2 lg:grid-cols-4 xl:grid-cols-6 3xl:grid-cols-12 guilds-list">
    <DashboardWidget link="test" linkHref="/guilds/748943581523345639/cases">
        <div>
            DashboardWidgetawd<br />
            ArrowRight32aw<br />
            lwaysawd<br />
                alwaysawdawd<br />
                awd

                alwaysawdawd<br />
                alwaysawdawd<br />
                awd<br />
                awd<br />

                alwaysawdawd<br />
                awd<br />
                awd<br />
                awd<br />

                alwaysawdawd<br />
                awd<br />
                awd<br />
                awd<br />

                alwaysawdawd<br />
                awd<br />
                awd<br />
                awd<br />

                alwaysawdawd<br />
                awd<br />
                awd<br />
                awd
        </div>
    </DashboardWidget>
    <DashboardWidget>hi</DashboardWidget>
    <DashboardWidget state={WidgetState.Permission}>hi</DashboardWidget>
    <DashboardWidget lowPaddingMode link="test" linkHref="/guilds/748943581523345639/cases" mode={WidgetMode.x2_1}>
        <div style="visibility: hidden" id="test" use:chart={options} />
    </DashboardWidget>
    <DashboardWidget state={WidgetState.Error}>hi</DashboardWidget>
    <DashboardWidget state={WidgetState.Loading}>hi</DashboardWidget>
    <DashboardWidget link="test" linkHref="/guilds/748943581523345639/cases" state={WidgetState.Empty}>hi</DashboardWidget>
    <DashboardWidget>hi</DashboardWidget>
    <DashboardWidget>hi</DashboardWidget>
    <DashboardWidget mode={WidgetMode.x2_1}>hi</DashboardWidget>
    <DashboardWidget link="test" linkHref="/guilds/748943581523345639/cases" mode={WidgetMode.x2_1} state={WidgetState.Error}>
        <div style="visibility: hidden" id="test" use:chart={options} /></DashboardWidget>
    <DashboardWidget>hi</DashboardWidget>
    <DashboardWidget>hi</DashboardWidget>
    <DashboardWidget link="test" linkHref="/guilds/748943581523345639/cases" mode={WidgetMode.x2_1} state={WidgetState.Empty}>
        <div style="visibility: hidden" id="test" use:chart={options} /></DashboardWidget>
    <DashboardWidget lowPaddingMode mode={WidgetMode.x2_1}>
        <div style="visibility: hidden" id="test" use:chart={options} /></DashboardWidget>
    <DashboardWidget>hi</DashboardWidget>
    <DashboardWidget>hi</DashboardWidget>
</div>
