<script lang="ts">
    import { WidgetMode } from "./../../../core/dashboard/WidgetMode";
    import DashboardWidget from "../../../core/dashboard/DashboardWidget.svelte";
    import { WidgetState } from "../../../core/dashboard/WidgetState";
    import type { IDashboardItem } from "../../../models/IDashboardItem";
    import { _ } from "svelte-i18n";
    import {
        guildDashboardEnableDragging,
        guildDashboardItems,
        guildDashboardToggledItems,
        visibleGuildDashboardItems,
    } from "../../../stores/dashboardItems";
    import type { Writable } from "svelte/store";
    import { writable } from "svelte/store";
    import { Modal } from "carbon-components-svelte";
    import { Toggle } from "carbon-components-svelte";

    export let dashboardItem: IDashboardItem;
    let widgetState = WidgetState.Normal;
    let localItems = [];

    const showModal: Writable<boolean> = writable(false);

    function openModal() {
        showModal.set(true);
        localItems = $guildDashboardItems;
        guildDashboardEnableDragging.set(false);
    }

    function onModalClose() {
        showModal.set(false);
    }

    function widgetIsActivated(item: IDashboardItem): boolean {
        return $guildDashboardToggledItems.length === 0 || $guildDashboardToggledItems.some((x) => x.id === item.id && x.enabled);
    }

    function widgetIsToggled(toggled: boolean, item: IDashboardItem) {
        guildDashboardToggledItems.update((n) => {
            if (toggled) {
                n.unshift({
                    id: item.id,
                    enabled: true,
                    sortOrder: -1,
                });
            } else {
                n = n.filter((x) => x.id !== item.id);
            }
            return n;
        });
    }
</script>

<DashboardWidget
    title={$_("widgets.dashboardconfig.title")}
    mode={dashboardItem.mode === WidgetMode.x1_1 ? WidgetMode.x1_1 : WidgetMode.x2_1}
    state={widgetState}>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.dashboardconfig.available")}
        </div>
        <div class="self-end">
            {$guildDashboardItems.filter(x => !x.fix).length}
        </div>
    </div>
    <div class="dash-widget-list-border flex flex-row items-center py-2" style="height: 2rem">
        <div class="grow">
            {$_("widgets.dashboardconfig.enabled")}
        </div>
        <div class="self-end">
            {$visibleGuildDashboardItems.filter(x => !x.fix).length}
        </div>
    </div>
    <div on:click={openModal}>open</div>
    <Toggle size="sm" labelText="" toggled={$guildDashboardEnableDragging} on:toggle={(e) => guildDashboardEnableDragging.set(e.detail.toggled)} />
</DashboardWidget>

<Modal size="sm" open={$showModal} modalHeading="Widgets" passiveModal on:close={onModalClose} on:submit={onModalClose}>
    <div class="flex flex-col">
        {#each localItems as item (item.id)}
            <div class="flex flex-row border-2 border-bottom mb-4">
                <div class="flex justify-center align-center mr-10 shrink-0">
                    <Toggle
                        title={item.fix ? 'This widget cannot be disabled.' : ''}
                        disabled={item.fix}
                        labelText=""
                        toggled={item.fix ? true : widgetIsActivated(item)}
                        on:toggle={(e) => {
                            widgetIsToggled(e.detail.toggled, item);
                        }} />
                </div>
                <div class="flex flex-col grow">
                    <div>
                        {$_(`widgets.${item.translationKey}.title`)}
                    </div>
                    <div style="color: var(--cds-text-02)">
                        {$_(`widgets.${item.translationKey}.widgetdescription`)}
                    </div>
                </div>
            </div>
        {/each}
    </div>
</Modal>
