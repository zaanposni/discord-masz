<script lang="ts">
    import { AspectRatio, Link } from "carbon-components-svelte";
    import { WidgetMode } from "./WidgetMode";
    import { WidgetState } from "./WidgetState";
    import UserSearch from "carbon-pictograms-svelte/lib/UserSearch.svelte";
    import BugVirusMalware from "carbon-pictograms-svelte/lib/BugVirusMalware.svelte";
    import Robot from "carbon-pictograms-svelte/lib/Robot.svelte";
    import MediaQuery from "../MediaQuery.svelte";

    export let mode: WidgetMode = WidgetMode.x1_1;
    export let state: WidgetState = WidgetState.Normal;
    let ratio;
    $: switch (mode) {
        case WidgetMode.x2_1:
            ratio = "2x1";
            break;
        default:
            ratio = "1x1";
            break;
    }

    export let title: string = "Title";
    export let link: string | null = null;
    export let linkHref: string | null = null;
    export let lowPaddingMode: boolean = false;
</script>

<style>
    .dash-widget {
        background-color: var(--cds-ui-01, #ffffff);
        color: var(--cds-text-01, #152935);
        font-size: 0.875rem;
        text-align: left;
        display: flex;
        flex-direction: column;
    }

    .dash-widget .dash-widget-title {
        border-bottom: 1px solid var(--cds-ui-03, #f0f3f6);
        height: 2.5rem;
        display: flex;
        align-items: center;
        padding: 0 1.5rem 0 1.5rem;
        font-weight: 700;
        flex-shrink: 0;
    }

    :global(.dash-widget .dash-widget-content .dash-widget-list-border:not(:last-child)) {
        border-bottom: 1px solid var(--cds-ui-03, #f0f3f6);
    }

    .dash-widget .dash-widget-content {
        padding: 1.5rem;
        display: flex;
        flex-direction: column;
        flex-grow: 1;
        flex-shrink: 0;
        height: calc(100% - 2.5rem);
        overflow: auto;
    }

    .dash-widget .dash-widget-content.padding-low {
        padding: 0 0.5rem 0.2rem 0.5rem;
        overflow: unset;
    }

    .dash-widget.unnormal .dash-widget-content {
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        color: var(--cds-text-02, #5a6872);
        font-size: 0.75rem;
        text-align: center;
    }
</style>

<!-- If we are on a small screen the aspect ratio should always be 1x1 to prevent "smaller neighbors" -->
<MediaQuery query="(min-width: 768px)" let:matches>
    <AspectRatio ratio={matches ? ratio : "1x1"} class={mode}>
        <div class="flex flex-col dash-widget h-full" class:unnormal={state !== WidgetState.Normal && state !== WidgetState.Loading}>
            <div class="dash-widget-title">
                {title}
                <div class="grow" />
                {#if link && linkHref}
                    <Link href={linkHref}>
                        {link}
                    </Link>
                {/if}
            </div>
            <div class="dash-widget-content" class:padding-low={lowPaddingMode}>
                {#if state === WidgetState.Normal}
                    <slot>Content</slot>
                {/if}
                {#if state === WidgetState.Loading}
                    <slot name="loading" />
                {/if}
                {#if state === WidgetState.Error}
                    <BugVirusMalware fill="var(--cds-danger)" />
                    <div class="h-4" />
                    <div>This widget can't be loaded at this time. Refresh the page to try again.</div>
                {/if}
                {#if state === WidgetState.Empty}
                    <Robot />
                    <div class="h-4" />
                    <slot name="empty">
                        <div>This is not the widget you are looking for.</div>
                    </slot>
                {/if}
                {#if state === WidgetState.Permission}
                    <UserSearch />
                    <div class="h-4" />
                    <div>You don't have permission to view this resource with this acocunt.</div>
                {/if}
            </div>
        </div>
    </AspectRatio>
</MediaQuery>
