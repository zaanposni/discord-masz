<script>
    import GuildCards from "../../components/guilds/GuildCards.svelte";
    import { authUser } from "../../stores/auth";
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

{#if $authUser}
    <h2>Admin guilds ({$authUser.adminGuilds?.length ?? 0})</h2>
    <GuildCards guilds={$authUser.adminGuilds} />

    <h2>Moderator guilds ({$authUser.modGuilds?.length ?? 0})</h2>
    <GuildCards guilds={$authUser.modGuilds} />

    <h2>Member guilds ({$authUser.memberGuilds?.length ?? 0})</h2>
    <GuildCards guilds={$authUser.memberGuilds} />

    <h2>Banned from guilds ({$authUser.bannedGuilds?.length ?? 0})</h2>
    <GuildCards guilds={$authUser.bannedGuilds} />
{/if}
