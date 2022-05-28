<script lang="ts">
    import { Add32 } from "carbon-icons-svelte";
    import { writable, Writable } from "svelte/store";
    import type { IDiscordGuild } from "../../../models/discord/IDiscordGuild";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import GuildCard from "../GuildCard.svelte";
    import { slide } from "svelte/transition";
    import { Button, Loading } from "carbon-components-svelte";
    import { _ } from "svelte-i18n";
    import { toastError } from "../../../services/toast/store";
    import EditGuildConfig from "./EditGuildConfig.svelte";
    import { applicationInfo } from "../../../stores/applicationInfo";

    const guilds: Writable<IDiscordGuild[]> = writable([]);
    const selectedGuild: Writable<IDiscordGuild | null> = writable(null);
    const selectedGuildLoading: Writable<boolean> = writable(true);
    const selectedGuildError: Writable<boolean> = writable(true);

    API.get("/discord/guilds", CacheMode.API_ONLY, false).then((res) => {
        guilds.set(res);
    });

    function selectGuild(guild: IDiscordGuild) {
        selectedGuild.set(guild);
        API.get(`/discord/guilds/${guild.id}`, CacheMode.API_ONLY, false)
            .then((res) => {
                selectedGuildLoading.set(false);
                selectedGuildError.set(false);
            })
            .catch(() => {
                selectedGuildLoading.set(false);
                selectedGuildError.set(true);
            });
    }

    function invite() {
        var win = window.open(
            `https://discord.com/oauth2/authorize?client_id=${$applicationInfo?.id}&permissions=8&scope=bot%20applications.commands&guild_id=${$selectedGuild.id}`,
            "Add bot to guild",
            "status=yes;width=150,height=400"
        );
        if (win === null) {
            toastError($_("guilds.add.windowfailed"));
            return;
        }
        var timer = setInterval(
            function (callback: any, id: string, context: any) {
                if (win?.closed) {
                    clearInterval(timer);
                    callback.bind(context, id)();
                }
            },
            500,
            selectGuild,
            $selectedGuild,
            this
        );
    }
</script>

{#if !$selectedGuild}
    <div class="grid gap-4 grid-cols-1 md:grid-cols-2 lg:grid-cols-4 3xl:grid-cols-8" transition:slide|local>
        {#each $guilds as guild (guild.id)}
            <GuildCard
                {guild}
                on:click={() => {
                    selectGuild(guild);
                }} />
        {/each}
    </div>
{:else}
    <div transition:slide|local>
        <GuildCard guild={$selectedGuild} />
        {#if $selectedGuildLoading}
            <div class="flex flex-col items-center justify-center w-full mt-8" transition:slide|local>
                <Loading withOverlay={false} />
            </div>
        {:else if $selectedGuildError}
            <div class="flex flex-col items-center justify-center w-full mt-8" transition:slide|local on:click={invite}>
                <Button icon={Add32}>{$_("guilds.add.invitefirst")}</Button>
            </div>
        {:else}
            <div class="mt-8" transition:slide|local>
                <EditGuildConfig addMode guildId={$selectedGuild.id} />
            </div>
        {/if}
    </div>
{/if}
