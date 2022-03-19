<script lang="ts">
    import GuildCards from "../../components/guilds/GuildCards.svelte";
    import { authUser } from "../../stores/auth";

    let noGuilds: boolean = false;
    $: noGuilds =
        $authUser?.adminGuilds?.length === 0 &&
        $authUser?.modGuilds?.length === 0 &&
        $authUser?.memberGuilds?.length === 0 &&
        $authUser?.bannedGuilds?.length === 0;
</script>

<style>
    h2 {
        padding-left: 1rem;
        margin-bottom: 0.5rem;
    }

    h2:not(:first-child) {
        margin-top: 1rem;
    }
</style>

{#if noGuilds}
    no guilds
{:else if $authUser}
    {#if $authUser.adminGuilds.length > 0}
        <h2>Admin guilds ({$authUser.adminGuilds?.length ?? 0})</h2>
        <GuildCards guilds={$authUser.adminGuilds} />
    {/if}
    {#if $authUser.modGuilds.length > 0}
        <h2>Moderator guilds ({$authUser.modGuilds?.length ?? 0})</h2>
        <GuildCards guilds={$authUser.modGuilds} />
    {/if}
    {#if $authUser.memberGuilds.length > 0}
        <h2>Guilds ({$authUser.memberGuilds?.length ?? 0})</h2>
        <GuildCards guilds={$authUser.memberGuilds} />
    {/if}
    {#if $authUser.bannedGuilds.length > 0}
        <h2>Banned guilds ({$authUser.bannedGuilds?.length ?? 0})</h2>
        <GuildCards guilds={$authUser.bannedGuilds} />
    {/if}
{/if}
