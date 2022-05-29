<script lang="ts">
    import { adminDashboardEnableDragging } from "./../../../stores/adminDashboardItems";
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";
    import type { IMASZVersion } from "../../../models/api/IMASZVersion";
    import { derived, Readable, Writable } from "svelte/store";
    import { writable } from "svelte/store";
    import { compare } from "compare-versions";
    import { APP_VERSION } from "../../../config";
    import { CircleSolid16, Launch16, LogoGithub16, NotificationFilled16 } from "carbon-icons-svelte";
    import { currentLanguage } from "../../../stores/currentLanguage";
    import moment from "moment";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import { Link, SkeletonText } from "carbon-components-svelte";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Normal;

    const remoteVersions: Writable<IMASZVersion[]> = writable([]);

    const newVersion: Readable<IMASZVersion> = derived(remoteVersions, (versions) => {
        if (versions.length === 0) {
            return null;
        }

        const remoteVersion = versions[0].tag.replace("a", "-alpha");
        const localVersion = APP_VERSION;

        if (compare(remoteVersion, localVersion, ">")) {
            return versions[0];
        }

        return null;
    });

    API.get("/meta/versions", CacheMode.PREFER_CACHE, true).then((response) => {
        remoteVersions.set(response);
    });
</script>

<DashboardWidget
    title={$_("widgets.adminversion.title")}
    mode={dashboardItem.mode}
    state={$adminDashboardEnableDragging ? WidgetState.Loading : widgetState}>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.adminversion.current")}
        </div>
        <div class="mr-1">
            {APP_VERSION}
        </div>
        <CircleSolid16 style="fill: var(--cds-ui-04)" />
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("nav.patchnotes")}
        </div>
        <Link href={`/patchnotes`} icon={Launch16} class="align-end" />
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">GitHub</div>
        <Link target="_blank" href={"https://github.com/zaanposni/discord-masz"} icon={LogoGithub16} class="align-end" />
    </div>
    {#if $newVersion}
        <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem; color: var(--cds-support-warning-inverse)">
            <div class="grow">
                {$_("widgets.adminversion.new")}
            </div>
            <div class="mr-1">
                {$newVersion.tag} ({moment($newVersion.createdAt).format($currentLanguage?.momentDateFormat ?? "DD.MM.YYYY")})
            </div>
            <NotificationFilled16 style="fill: var(--cds-support-warning-inverse)" />
        </div>
    {/if}
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
