<script lang="ts">
    import { goto } from "@roxi/routify";
    import type { IDiscordGuild } from "../../models/discord/IDiscordGuild";

    import GuildIcon from "../discord/GuildIcon.svelte";

    export let guild: IDiscordGuild;
</script>

<div class="flex flex-row flex-nowrap masz-card cursor-pointer" on:click>
    <div class="shrink-0">
        <GuildIcon {guild} />
    </div>
    <h3 class="ml-3" style="word-break: break-word;">{guild.name}</h3>
</div>
