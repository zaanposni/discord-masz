<script lang="ts">
    import { ComboBox } from "carbon-components-svelte";

    export let id = "";
    export let autoSelect: boolean = true;
    export let warnText = "";
    export let invalidText = "";
    export let disabled: boolean = false;
    export let titleText: string = "";
    export let placeholder: string = "";
    export let selectedId;
    export let value;
	export let ref;
    export let items: { id: string; text: string; custom?: boolean }[] = [];

    export let valueMatchCustomValue = (value: string) => {
        return !!value;
    };

    export let shouldFilterItem = (item, value) => {
        if (!value) return true;
        value = value.toLowerCase();
        return item.text.toLowerCase().includes(value);
    };

    let debounce;
    function onKeyDown(event) {
        if (event.key.length === 1 || event.key === "Backspace" || event.key === "Delete") {
            if (debounce) {
                clearTimeout(debounce);
            }
            debounce = setTimeout(() => {
                if (valueMatchCustomValue(value)) {
                    const index = items.findIndex((x) => x.custom);
                    if (index !== -1) {
                        items[index].text = value;
						items[index].id = value;
                    } else {
                        items.unshift({ id: value, text: value, custom: true });
                    }
                    if (autoSelect) {
                        selectedId = value;
                    }
                }
            }, 50);
        }
    }

    function onClear() {
        items = items.filter((x) => !x.custom);
    }

    $: value ? undefined : onClear();
</script>

<ComboBox
    {id}
	bind:this={ref}
    {titleText}
    {placeholder}
    {items}
    {warnText}
    warn={!!warnText}
    {invalidText}
    invalid={!!invalidText}
    {shouldFilterItem}
    {disabled}
    bind:selectedId
    bind:value
    on:select
	on:clear={onClear}
    on:keydown={onKeyDown} />
