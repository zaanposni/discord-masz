<script lang="ts">
    import GuildIcon from "./../../discord/GuildIcon.svelte";
    import { Button, DataTable, Toolbar, ToolbarContent, ToolbarSearch, Loading, Row } from "carbon-components-svelte";
    import moment from "moment";
    import { Add32, Add24 } from "carbon-icons-svelte";
    import { writable, Writable } from "svelte/store";
    import type { IDiscordGuild } from "../../../models/discord/IDiscordGuild";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import GuildCard from "../GuildCard.svelte";
    import { slide } from "svelte/transition";
    import { _ } from "svelte-i18n";
    import { toastError } from "../../../services/toast/store";
    import EditGuildConfig from "./EditGuildConfig.svelte";
    import { applicationInfo } from "../../../stores/applicationInfo";
    import MediaQuery from "../../../core/MediaQuery.svelte";

    const guilds: Writable<IDiscordGuild[]> = writable([]);
    const selectedGuild: Writable<IDiscordGuild | null> = writable(null);
    const selectedGuildLoading: Writable<boolean> = writable(true);
    const selectedGuildError: Writable<boolean> = writable(true);

    let headers = [];
    $: headers = [
        { key: "name", value: $_("guilds.add.name"), minWidth: "60px" },
        { key: "id", value: $_("guilds.add.id"), minWidth: "60px" },
        { key: "action", value: $_("guilds.add.add"), width: "100px" },
    ];

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
    <MediaQuery query="(min-width: 768px)" let:matches>
        <div class="flex flex-col {matches ? 'w-1/2' : ''}">
            <div transition:slide|local>
                <DataTable zebra title={$_("guilds.add.button")} {headers} bind:rows={$guilds}>
                    <Toolbar>
                        <ToolbarContent>
                            <ToolbarSearch
                                persistent
                                shouldFilterRows={(row, value) => {
                                    return row.name.toLowerCase().includes(value.toLowerCase()) || row.id.toLowerCase().includes(value.toLowerCase());
                                }} />
                        </ToolbarContent>
                    </Toolbar>
                    <svelte:fragment slot="cell" let:row let:cell>
                        {#if cell.key === "action"}
                            <Button on:click={() => selectGuild(row)} icon={Add24} iconDescription={$_("guilds.add.button")} />
                        {:else if cell.key === "name"}
                            <div class="flex flex-row items-center">
                                <GuildIcon guild={row} />
                                <span class="ml-4">{cell.value}</span>
                            </div>
                        {:else}
                            {cell.value}
                        {/if}
                    </svelte:fragment>
                </DataTable>
            </div>
        </div>
    </MediaQuery>
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
