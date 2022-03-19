<script lang="ts">
	import { currentParams } from './../../../stores/currentParams';

    $: console.log($currentParams);
</script>

{$currentParams.guildId} dashboard
{$currentParams.guild?.name}
