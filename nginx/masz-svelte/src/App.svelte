<script lang="ts">
	import { Router } from "@roxi/routify";
	import { routes } from "../.routify/routes";

	import "carbon-components-svelte/css/all.css";

	import { Accordion, AccordionItem } from "carbon-components-svelte";
	import Add32 from "carbon-icons-svelte/lib/Add32";

	import Theme from "./components/nav/theme.svelte";
</script>

<style global lang="postcss">
    @tailwind base;
    @tailwind components;
    @tailwind utilities;
</style>

<main>
	<Theme />
	<Router {routes} />
	<i class="material-icons">home</i>
	<Add32 title="Add" class="my-5" />
	<Accordion>
	  <AccordionItem title="Section 1" open> Content 1 </AccordionItem>
	  <AccordionItem title="Section 2"> Content 2 </AccordionItem>
	  <AccordionItem title="Section 3"> Content 3 </AccordionItem>
	</Accordion>
</main>
