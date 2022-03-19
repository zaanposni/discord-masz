<script lang="ts">
	import { params } from '@roxi/routify';

    $: console.log($params);
</script>

{$params.guildId} cases
