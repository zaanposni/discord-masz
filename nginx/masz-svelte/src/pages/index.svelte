<script lang="ts">
	import { isLoggedIn } from './../stores/auth';
	import { goto } from '@roxi/routify';
    $goto($isLoggedIn ? "/guilds" : "/login");
</script>
