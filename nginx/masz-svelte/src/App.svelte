<script lang="ts">
	import { Router } from "@roxi/routify";
	import { routes } from "../.routify/routes";
	import { Button, Autocomplete } from "attractions";

	async function* getOptions(text) {
		yield [
			{ name: text, details: "Optional" },
			{ name: `it highlights the match: ${text}` },
		];
	}
</script>

<style global lang="postcss">
    @tailwind base;
    @tailwind components;
    @tailwind utilities;
</style>

<main>
	<Router {routes} />
	<Button filled class="mb-3">click me</Button>
	<i class="material-icons">home</i>

	<Autocomplete {getOptions} minSearchLength={0} />
</main>
