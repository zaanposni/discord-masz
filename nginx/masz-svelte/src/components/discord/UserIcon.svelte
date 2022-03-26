<script lang="ts">
    import type { IDiscordUser } from "../../models/discord/IDiscordUser";
    import { authUser } from "../../stores/auth";

    let classes;
    export { classes as class };
    export let user: IDiscordUser | string | null = null;

    const srcFallBack = "https://cdn.discordapp.com/embed/avatars/1.png";
    let src = srcFallBack;
    let alt = "user avatar";

    function calculateImage(newUser) {
        user = newUser;
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
    calculateImage(user);
</script>

<img
    height="32"
    width="32"
    class="rounded-full {classes}"
    {alt}
    {src}
    on:error={() => {
        src = srcFallBack;
    }} />
