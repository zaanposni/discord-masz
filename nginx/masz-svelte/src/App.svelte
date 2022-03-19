<script lang="ts">
    import { Router } from "@roxi/routify";
    import { routes } from "../.routify/routes";
    import "carbon-components-svelte/css/all.css";
    import Toasts from "./services/toast/toasts.svelte";
    import { currentTheme } from "./stores/theme";

    setTimeout(() => {
        currentTheme.set(document.getElementsByTagName("html")[0].getAttribute("theme"));
    }, 100);
</script>

<style global lang="postcss">
    @tailwind base;
    @tailwind components;
    @tailwind utilities;
</style>

<Toasts />
<Router {routes} />
