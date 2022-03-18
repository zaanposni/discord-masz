<script lang="ts">
	import { toastWarning } from './../services/toast/store';
	import { authUser } from './../stores/auth';
    import { Button } from "carbon-components-svelte";

    function login() {
        authUser.set({} as any);
    }
</script>

login 1u82739127398

<Button on:click={login}>Login</Button>
<Button on:click={() => { toastWarning("dawidhauw dhauwdh auiwdh auiwdawd."); }}>Test</Button>
<Button on:click={() => { toastWarning("dawidhauw dhauwdh auiwdh auiwdawd.", "test", 5000, false); }}>Test2</Button>

