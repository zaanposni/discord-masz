<script lang="ts">
import { goto } from "@roxi/routify";

    import type { IDiscordGuild } from "../../models/discord/IDiscordGuild";
    import GuildCard from "./GuildCard.svelte";

    export let guilds: IDiscordGuild[] = [];
    export let privileged: boolean = false;
</script>

<div class="grid gap-4 grid-cols-1 md:grid-cols-2 lg:grid-cols-4 3xl:grid-cols-8">
    {#each guilds as guild}
        <GuildCard {guild} on:click={() => {
            $goto(privileged ? `/guilds/${guild.id}` : `/guilds/${guild.id}/cases`);
        }} />
    {/each}
</div>
