<script lang="ts">
    import type { IDiscordUser } from "../../models/discord/IDiscordUser";

    let classes;
    export let size: number = 32;
    export { classes as class };
    export let user: IDiscordUser | string | null = null;

    const srcFallBack = "https://cdn.discordapp.com/embed/avatars/1.png";
    let src = srcFallBack;
    let alt = "user avatar";

    function calculateImage(newUser) {
        if (newUser && typeof newUser === "object") {
            alt = `${newUser.username}#${newUser.discriminator}`;
            src = newUser.imageUrl;
        } else {
            alt = "user avatar";
            if (typeof newUser === "string") {
                src = newUser;
            } else {
                src = srcFallBack;
            }
        }
    }
    $: calculateImage(user);
</script>

<img
    height="{size}"
    width="{size}"
    class="rounded-full {classes}"
    {alt}
    {src}
    on:error={() => {
        src = srcFallBack;
    }} />
