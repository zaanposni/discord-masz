<script lang="ts">
  import { Router, goto, afterPageLoad } from "@roxi/routify";
  import { routes } from "../.routify/routes";
  import "carbon-components-svelte/css/all.css";
  import Toasts from "./services/toast/toasts.svelte";
</script>

<Toasts />
<Router {routes} />

<style global lang="postcss">
  @tailwind base;
  @tailwind components;
  @tailwind utilities;
</style>
