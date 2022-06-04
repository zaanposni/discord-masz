<script lang="ts">
    import { adminDashboardEnableDragging } from "./../../../stores/adminDashboardItems";
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";
    import { writable } from "svelte/store";
    import type { Writable } from "svelte/store";
    import { SkeletonText } from "carbon-components-svelte";
    import type { IAdminStats } from "../../../models/api/IAdminStats";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import { chart } from "../../../core/charts/chart.js";
import { CircleSolid16 } from "carbon-icons-svelte";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Loading;

    const adminStats: Writable<IAdminStats> = writable(null);

    const userRegex = /^u:(\d+)$/;
    const guildRegex = /^g:(\d+)$/;
    const guildBanRegex = /^g:(\d+):b:(\d+)$/;
    const guildMemberRegex = /^g:(\d+):m:(\d+)$/;
    const dmChannelRegex = /^d:(\d+)$/;
    const tokenUserRegex = /^t:(\d+)$/;

    let userCount = 0;
    let guildCount = 0;
    let guildBanCount = 0;
    let guildMemberCount = 0;
    let dmChannelCount = 0;
    let tokenUserCount = 0;

    API.get("/meta/adminstats", CacheMode.API_ONLY, false)
        .then((res: IAdminStats) => {
            adminStats.set(res);

            for (let i = 0; i < res.cachedDataFromDiscord.length; i++) {
                let cache = res.cachedDataFromDiscord[i];
                if (cache.match(userRegex)) {
                    userCount++;
                } else if (cache.match(guildRegex)) {
                    guildCount++;
                } else if (cache.match(guildBanRegex)) {
                    guildBanCount++;
                } else if (cache.match(guildMemberRegex)) {
                    guildMemberCount++;
                } else if (cache.match(dmChannelRegex)) {
                    dmChannelCount++;
                } else if (cache.match(tokenUserRegex)) {
                    tokenUserCount++;
                }
            }
            widgetState = WidgetState.Normal;
        })
        .catch(() => {
            widgetState = WidgetState.Error;
        });
</script>

<DashboardWidget
    title={$_("widgets.admincache.title")}
    mode={dashboardItem.mode}
    state={$adminDashboardEnableDragging ? WidgetState.Loading : widgetState}>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.admincache.users")}
        </div>
        <div class="mr-1">
            {userCount}
        </div>
        <CircleSolid16 style="fill: var(--cds-ui-04)" />
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.admincache.guilds")}
        </div>
        <div class="mr-1">
            {guildCount}
        </div>
        <CircleSolid16 style="fill: var(--cds-ui-04)" />
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.admincache.bans")}
        </div>
        <div class="mr-1">
            {guildBanCount}
        </div>
        <CircleSolid16 style="fill: var(--cds-ui-04)" />
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.admincache.members")}
        </div>
        <div class="mr-1">
            {guildMemberCount}
        </div>
        <CircleSolid16 style="fill: var(--cds-ui-04)" />
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.admincache.dmchannels")}
        </div>
        <div class="mr-1">
            {dmChannelCount}
        </div>
        <CircleSolid16 style="fill: var(--cds-ui-04)" />
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.admincache.tokens")}
        </div>
        <div class="mr-1">
            {tokenUserCount}
        </div>
        <CircleSolid16 style="fill: var(--cds-ui-04)" />
    </div>
    <div slot="loading">
        <div class="dash-widget-list-border flex items-center" style="height: 2rem">
            <SkeletonText />
        </div>
        <div class="dash-widget-list-border flex items-center" style="height: 2rem">
            <SkeletonText />
        </div>
        <div class="dash-widget-list-border flex items-center" style="height: 2rem">
            <SkeletonText />
        </div>
    </div>
</DashboardWidget>
