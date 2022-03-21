<script lang="ts">
    import { currentParams } from "./../../../stores/currentParams";
    import { _, locale } from "svelte-i18n";
    import type { IAuthUser } from "../../../models/IAuthUser";
    import type { IRouteParams } from "../../../models/IRouteParams";
    import { goto } from "@roxi/routify";
    import { authUser } from "../../../stores/auth";

    function checkForModOrHigher(user: IAuthUser, params: IRouteParams) {
        if (user && params?.guildId) {
            if (!user.adminGuilds.map((x) => x.id).includes(params.guildId) && !user.modGuilds.map((x) => x.id).includes(params.guildId)) {
                $goto("/guilds/" + $currentParams.guildId + "/cases");
            }
        }
    }
    $: checkForModOrHigher($authUser, $currentParams);
</script>

{$currentParams.guildId} dashboard
{$currentParams.guild?.name}

{$_("Forms.FieldRequired")}

<div
    on:click={() => {
        locale.set("de");
    }}>
    test
</div>

<div
    on:click={() => {
        locale.set("en");
    }}>
    test2
</div>
