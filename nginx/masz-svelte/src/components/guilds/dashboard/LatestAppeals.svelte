<script lang="ts">
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";
    import API from "../../../services/api/api";
    import { currentParams } from "../../../stores/currentParams";
    import { CacheMode } from "../../../services/api/CacheMode";
    import { Link, SkeletonText, Truncate } from "carbon-components-svelte";
    import UserIcon from "../../discord/UserIcon.svelte";
    import { Launch20 } from "carbon-icons-svelte";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import { guildDashboardEnableDragging } from "../../../stores/dashboardItems";
    import type { IAppealView } from "../../../models/api/IAppealView";
    import type { IAppealTable } from "../../../models/api/IAppealTable";
    import AppealStatusTag from "../../api/AppealStatusTag.svelte";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Loading;

    $: $currentParams?.guildId && !$guildDashboardEnableDragging ? loadData() : null;
    let appeals: IAppealView[] = [];
    function loadData() {
        widgetState = WidgetState.Loading;
        API.post(`/guilds/${$currentParams.guildId}/appeal/table`, {}, CacheMode.PREFER_CACHE, true)
            .then((response: IAppealTable) => {
                appeals = response.appealViews.slice(0, 5);
                widgetState = response.fullSize ? WidgetState.Normal : WidgetState.Empty;
            })
            .catch(() => {
                widgetState = WidgetState.Error;
            });
    }
</script>

<MediaQuery query="(min-width: 768px)" let:matches>
    <DashboardWidget
        title={$_("widgets.latestappeals.title")}
        mode={dashboardItem.mode}
        state={$guildDashboardEnableDragging ? WidgetState.Loading : widgetState}>
        {#each appeals as appeal}
            <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2.5rem">
                <div class="grow shrink-0 mr-1">
                    <UserIcon user={appeal.user} />
                </div>
                {#if matches}
                    <AppealStatusTag class="grow shrink-0 mr-1" {appeal} />
                {/if}
                <Truncate title={appeal.userId}>
                    {appeal.user?.username || appeal.username}#{appeal.user?.discriminator || appeal.discriminator}
                </Truncate>
                <Link href={`/guilds/${appeal.guildId}/appeals/${appeal.id}`} icon={Launch20} class="align-end" />
            </div>
        {/each}
        <div slot="loading">
            <div class="dash-widget-list-border flex items-center" style="height: 2.5rem">
                <SkeletonText />
            </div>
            <div class="dash-widget-list-border flex items-center" style="height: 2.5rem">
                <SkeletonText />
            </div>
            <div class="dash-widget-list-border flex items-center" style="height: 2.5rem">
                <SkeletonText />
            </div>
            <div class="dash-widget-list-border flex items-center" style="height: 2.5rem">
                <SkeletonText />
            </div>
            <div class="dash-widget-list-border flex items-center" style="height: 2.5 rem">
                <SkeletonText />
            </div>
        </div>
        <div slot="empty">
            {$_("widgets.latestappeals.empty")}
        </div>
    </DashboardWidget>
</MediaQuery>
