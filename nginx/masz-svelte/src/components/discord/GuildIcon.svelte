<script lang="ts">
    import type { IDiscordGuild } from "../../models/discord/IDiscordGuild";

    export let guild: IDiscordGuild | string | null = null;

    const srcFallBack = "https://cdn.discordapp.com/embed/avatars/1.png";
    let src = srcFallBack;
    let alt = "guildicon";
    let characterMode: string = "";

    function calculateImage(newGuild) {
        guild = newGuild;
        if (newGuild && typeof newGuild === "object") {
            alt = `${newGuild.name} (${newGuild.id})`;
            if (newGuild.iconUrl) {
                src = newGuild.iconUrl;
            } else {
                characterMode = newGuild.name.charAt(0).toUpperCase();
            }
        } else {
            alt = "guildicon";
            if (typeof newGuild === "string") {
                src = newGuild;
            } else {
                src = srcFallBack;
            }
        }
    }
    calculateImage(guild);
</script>

{#if characterMode}
    <div class="flex items-center justify-center align-baseline rounded-full border-solid border-2 border-gray-700" style="width: 40px; height: 40px">
        {characterMode}
    </div>
{:else}
    <img
        height="40"
        width="40"
        class="rounded-full"
        {alt}
        {src}
        on:error={() => {
            src = srcFallBack;
        }} />
{/if}
