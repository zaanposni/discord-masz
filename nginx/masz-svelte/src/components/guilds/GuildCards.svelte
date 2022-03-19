<script lang="ts">
    import type { IDiscordGuild } from "../../models/discord/IDiscordGuild";
    import GuildCard from "./GuildCard.svelte";

    export let guilds: IDiscordGuild[] = [];
</script>

<div class="grid gap-4 grid-cols-1 md:grid-cols-2 lg:grid-cols-4 3xl:grid-cols-8 guilds-list">
    {#each guilds as guild}
        <GuildCard {guild} />
    {/each}
</div>
