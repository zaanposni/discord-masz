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
    <DashboardWidget link="test" linkHref="/guilds/748943581523345639/cases">wadwdawdaw</DashboardWidget>
    <DashboardWidget>hi</DashboardWidget>
    <DashboardWidget state={WidgetState.Permission}>hi</DashboardWidget>
    <DashboardWidget link="test" linkHref="/guilds/748943581523345639/cases" mode={WidgetMode.x2_1}>hi</DashboardWidget>
    <DashboardWidget state={WidgetState.Error}>hi</DashboardWidget>
    <DashboardWidget state={WidgetState.Loading}>hi</DashboardWidget>
    <DashboardWidget link="test" linkHref="/guilds/748943581523345639/cases" state={WidgetState.Empty}>hi</DashboardWidget>
    <DashboardWidget>hi</DashboardWidget>
    <DashboardWidget>hi</DashboardWidget>
    <DashboardWidget mode={WidgetMode.x2_1}>hi</DashboardWidget>
    <DashboardWidget link="test" linkHref="/guilds/748943581523345639/cases" mode={WidgetMode.x2_1} state={WidgetState.Error}>hi</DashboardWidget>
    <DashboardWidget>hi</DashboardWidget>
    <DashboardWidget>hi</DashboardWidget>
    <DashboardWidget link="test" linkHref="/guilds/748943581523345639/cases" mode={WidgetMode.x2_1} state={WidgetState.Empty}>hi</DashboardWidget>
    <DashboardWidget mode={WidgetMode.x2_1}>hi</DashboardWidget>
    <DashboardWidget>hi</DashboardWidget>
    <DashboardWidget>hi</DashboardWidget>
</div>
