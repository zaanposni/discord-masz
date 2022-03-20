<script lang="ts">
    import { createEventDispatcher } from "svelte";
    import { HeaderPanelLink, Truncate } from "carbon-components-svelte";

    export let item: typeof import("svelte").SvelteComponent;
    export let text: string;

    const eventDispatcher = createEventDispatcher();
</script>

<HeaderPanelLink on:click>
    <div class="flex flex-row flex-nowrap">
        <svelte:component
            this={item}
            on:click={(e) => {
                eventDispatcher("iconClick", e);
                e.stopPropagation();
            }} />
        <Truncate class="ml-1" title={text}>{text}</Truncate>
    </div>
</HeaderPanelLink>
