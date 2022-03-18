<script lang="ts">
  import { Router, goto, afterPageLoad } from "@roxi/routify";
  import { routes } from "../.routify/routes";
  import "carbon-components-svelte/css/all.css";
</script>

<main>
  <Router {routes} />
</main>

<style global lang="postcss">
  @tailwind base;
  @tailwind components;
  @tailwind utilities;
</style>
