<script lang="ts">
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";
    import API from "../../../services/api/api";
    import { currentParams } from "../../../stores/currentParams";
    import { CacheMode } from "../../../services/api/CacheMode";
    import { Link, SkeletonText, Truncate } from "carbon-components-svelte";
    import PunishmentTag from "../../api/PunishmentTag.svelte";
    import UserIcon from "../../discord/UserIcon.svelte";
    import { Launch20 } from "carbon-icons-svelte";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import { guildDashboardEnableDragging } from "../../../stores/dashboardItems";
    import type { IScheduledMessage } from "../../../models/api/IScheduledMessage";
    import ScheduledMessageStatusTag from "../../api/ScheduledMessageStatusTag.svelte";
    import moment from "moment";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Loading;

    $: $currentParams?.guildId && !$guildDashboardEnableDragging ? loadData() : null;
    let messages: IScheduledMessage[] = [];
    function loadData() {
        widgetState = WidgetState.Loading;
        API.get(`/guilds/${$currentParams.guildId}/scheduledmessages/pending`, CacheMode.PREFER_CACHE, true)
            .then((response: IScheduledMessage[]) => {
                messages = response.slice(0, 8);
                widgetState = response.length ? WidgetState.Normal : WidgetState.Empty;
            })
            .catch(() => {
                widgetState = WidgetState.Error;
            });
    }
</script>

<MediaQuery query="(min-width: 768px)" let:matches>
    <DashboardWidget
        title={$_("widgets.guildpendingmessages.title")}
        mode={dashboardItem.mode}
        state={$guildDashboardEnableDragging ? WidgetState.Loading : widgetState}>
        {#each messages as message}
            <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2.5rem">
                <div class="grow shrink-0 mr-1">
                    <UserIcon user={message.lastEdited} />
                </div>
                {#if matches}
                    <ScheduledMessageStatusTag class="grow shrink-0 mr-1" {message} />
                {/if}
                <Truncate title={message.name}>
                    {message.scheduledFor.format("DD MMM HH:mm")}
                    {message.channel ? `#${message.channel.name}: ` : ""}
                    {message.name}
                </Truncate>
                <Link href={`/guilds/${message.guildId}/messages`} icon={Launch20} class="align-end" />
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
            {$_("widgets.guildpendingmessages.empty")}
        </div>
    </DashboardWidget>
</MediaQuery>
