<script lang="ts">
	import { authUser } from './../stores/auth';
    import { Button } from "carbon-components-svelte";

    function login() {
        authUser.set({} as any);
    }
</script>

login 1u82739127398

<Button on:click={login} >Login</Button>

