<script lang="ts">
	import { ComboBox } from "carbon-components-svelte";

	let warnText = "";

	let selectedId;
	let value;
	let items = [
	  { id: 1, text: "Email" },
	  { id: 2, text: "Fax" },
	];

	function shouldFilterItem(item, value) {
	  	if (!value) return true;
	  	return item.text.toLowerCase().includes(value.toLowerCase());
	}

	let debounce;
	function onKeyDown(event) {
		if (event.key.length === 1) {
			if (debounce) {
				clearTimeout(debounce);
			}
			debounce = setTimeout(() => {
				const index = items.findIndex(x => x.id === 0);
				if (index !== -1) {
					items[index].text = value;
				} else {
					items.unshift({ id: 0, text: value });
				}
			}, 300);
		}
	}

	function onSelect() {
		// if selected id is 0
		if (selectedId === 0) {
			warnText = "This is a custom item.";
		}
	}

	function onClear() {
		warnText = "";
		items = items.filter(x => x.id !== 0);
	}

	$: value ? undefined: onClear();
</script>

<ComboBox
	titleText="Contact"
	placeholder="Select contact method"
	{items}
	{warnText}
	warn={!!warnText}
	{shouldFilterItem}
	bind:selectedId
	bind:value
	on:select={onSelect}
	on:keydown={onKeyDown}
/>
