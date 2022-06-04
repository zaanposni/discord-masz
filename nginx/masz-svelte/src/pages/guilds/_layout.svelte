<script lang="ts">
    import { currentParams } from "./../../stores/currentParams";
    import { goto } from "@roxi/routify";
    import { favoriteGuild, firstVisitOnGuildList } from "../../stores/favoriteGuild";
    import { authUser } from "../../stores/auth";

    function checkForFavoriteGuild() {
        if ($favoriteGuild) {
            const guild =
                $authUser?.adminGuilds?.find((g) => g.id === $favoriteGuild) ||
                $authUser?.modGuilds?.find((g) => g.id === $favoriteGuild) ||
                $authUser?.memberGuilds?.find((g) => g.id === $favoriteGuild) ||
                $authUser?.bannedGuilds?.find((g) => g.id === $favoriteGuild);
            if (guild && !$currentParams.guildId) {
                $goto(`/guilds/${guild.id}`);
            }
        }
    }

    $firstVisitOnGuildList ? checkForFavoriteGuild() : firstVisitOnGuildList.set(false);
</script>

<slot />
