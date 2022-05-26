<script lang="ts">
    import Patchnotes from "../components/nav/patchnotes/Patchnotes.svelte";
    import { navConfig } from "../stores/nav";

    navConfig.set({
        enabled: false,
        items: [],
    });
</script>

<Patchnotes />
