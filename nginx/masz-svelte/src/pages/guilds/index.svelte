<script lang="ts">
	import { Add32 } from 'carbon-icons-svelte';
    import { url } from "@roxi/routify";
    import { Button } from "carbon-components-svelte";
    import GuildCards from "../../components/guilds/GuildCards.svelte";
    import { authUser, anyGuilds } from "../../stores/auth";
    import { navConfig } from "../../stores/nav";
    import { _ } from "svelte-i18n";

    navConfig.set({
        enabled: false,
        items: [],
    });
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

{#if $authUser?.isAdmin}
    <Button icon={Add32} href={$url(`/guilds/new`)}>{$_("guilds.add.button")}</Button>
{/if}

{#if !$anyGuilds}
    no guilds
{:else if $authUser}
    {#if $authUser?.adminGuilds?.length ?? 0 !== 0}
        <h2>{$_('guilds.list.admin')} ({$authUser.adminGuilds?.length ?? 0})</h2>
        <GuildCards guilds={$authUser.adminGuilds} privileged />
    {/if}
    {#if $authUser?.modGuilds?.length ?? 0 !== 0}
        <h2>{$_('guilds.list.mod')} ({$authUser.modGuilds?.length ?? 0})</h2>
        <GuildCards guilds={$authUser.modGuilds} privileged />
    {/if}
    {#if $authUser?.memberGuilds?.length ?? 0 !== 0}
        <h2>{$_('guilds.list.guild')} ({$authUser.memberGuilds?.length ?? 0})</h2>
        <GuildCards guilds={$authUser.memberGuilds} />
    {/if}
    {#if $authUser?.bannedGuilds?.length ?? 0 !== 0}
        <h2>{$_('guilds.list.banned')} ({$authUser.bannedGuilds?.length ?? 0})</h2>
        <GuildCards guilds={$authUser.bannedGuilds} />
    {/if}
{/if}
