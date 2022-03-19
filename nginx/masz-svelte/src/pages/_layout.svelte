<script lang="ts">
    import { goto, url } from "@roxi/routify";
    import { shortcut } from "../utils/shortcut.js";
    import {
        Header,
        SideNav,
        SideNavItems,
        SideNavMenu,
        SideNavMenuItem,
        SideNavLink,
        SideNavDivider,
        SkipToContent,
        Content,
        HeaderUtilities,
        HeaderGlobalAction,
        HeaderPanelLink,
        HeaderPanelLinks,
        HeaderAction,
        HeaderPanelDivider,
        HeaderSearch,
    } from "carbon-components-svelte";
    import Fade16 from "carbon-icons-svelte/lib/Fade16";
    import SettingsAdjust20 from "carbon-icons-svelte/lib/SettingsAdjust20";
    import UserIcon from "../components/discord/UserIcon.svelte";
    import { showUserSettings } from "../components/nav/store";
    import { Truncate } from "carbon-components-svelte";
    import Usersettings from "../components/nav/usersettings.svelte";
    import { APP_NAME, APP_VERSION } from "../config";
    import type { IDiscordGuild } from "../models/discord/IDiscordGuild";
    import { authUser, isLoggedIn } from "../stores/auth";
    import { currentParams } from "../stores/currentParams";
    import Star24 from "carbon-icons-svelte/lib/Star24";
    import StarFilled24 from "carbon-icons-svelte/lib/StarFilled24";
    import { favoriteGuild } from "../stores/favoriteGuild";

    if (window.location.pathname !== "/login" && !$isLoggedIn) {
        $goto("/login", { returnUrl: window.location.pathname });
    }

    let isSideNavOpen = false;
    let userIsOpen = false;
    let switcherIsOpen = false;

    let guilds: IDiscordGuild[] = [];
    $: guilds =
        $authUser?.adminGuilds?.concat(
            $authUser?.modGuilds,
            $authUser?.memberGuilds,
            $authUser?.bannedGuilds
        ) || [];

    function removeFavorite(guildId: string) {
        favoriteGuild.set("");
    }

    function changeFavorite(guildId: string) {
        favoriteGuild.set(guildId);
    }

    const data = [];
    for (let i = 0; i < 100; i++) {
        data.push({
            href: "/",
            text: "Kubernetes Service",
            description:
                "Deploy secure, highly available apps in a native Kubernetes experience. IBM Cloud Kubernetes Service creates a cluster of compute hosts and deploys highly available containers.",
        });
    }

    let value = "";
    let searchActive = false;

    $: lowerCaseValue = value.toLowerCase();
    $: results =
        value.length > 0
            ? data.filter((item) => {
                  return (
                      item.text.toLowerCase().includes(lowerCaseValue) ||
                      item.description.includes(lowerCaseValue)
                  );
              })
            : [];

    function activateSearch() {
        if ($isLoggedIn) {
            searchActive = true;
        }
    }
</script>

<Usersettings />

<div
    on:click={activateSearch}
    use:shortcut={{ control: true, code: "KeyK" }}
    use:shortcut={{ shift: true, code: "KeyK" }}
    use:shortcut={{ control: true, code: "KeyF" }}
    use:shortcut={{ shift: true, code: "KeyF" }}
/>
<Header company={APP_NAME} platformName={APP_VERSION} bind:isSideNavOpen>
    <svelte:fragment slot="skip-to-content">
        <SkipToContent />
    </svelte:fragment>
    <HeaderUtilities>
        {#if $isLoggedIn}
            <HeaderSearch
                bind:value
                bind:active={searchActive}
                placeholder="Search services"
                {results}
                on:select={(e) => {
                    console.log("hi", e);
                }}
            />
        {/if}
        <HeaderGlobalAction
            aria-label="Settings"
            icon={SettingsAdjust20}
            on:click={() => {
                showUserSettings.set(true);
            }}
        />
        {#if $isLoggedIn}
            <HeaderAction bind:isOpen={userIsOpen} icon={UserIcon}>
                <HeaderPanelLinks>
                    <HeaderPanelLink>Switcher item 1</HeaderPanelLink>
                </HeaderPanelLinks>
            </HeaderAction>
            <HeaderAction bind:isOpen={switcherIsOpen}>
                <HeaderPanelLinks>
                    <HeaderPanelLink href={$url("/guilds")}>
                        All guilds
                    </HeaderPanelLink>
                    <HeaderPanelDivider />
                    {#each guilds as guild (guild.id)}
                        <HeaderPanelLink
                            on:click={() => {
                                $goto(`/guilds/${guild.id}`);
                            }}
                        >
                            <div class="flex flex-row flex-nowrap">
                                {#if $favoriteGuild === guild.id}
                                    <StarFilled24
                                        class="mr-1"
                                        on:click={(e) => {
                                            removeFavorite(guild.id);
                                            e.stopPropagation();
                                        }}
                                    />
                                {:else}
                                    <Star24
                                        class="mr-1"
                                        on:click={(e) => {
                                            changeFavorite(guild.id);
                                            e.stopPropagation();
                                        }}
                                    />
                                {/if}
                                <Truncate>
                                    {guild.name}
                                </Truncate>
                            </div>
                        </HeaderPanelLink>
                    {/each}
                </HeaderPanelLinks>
            </HeaderAction>
        {/if}
    </HeaderUtilities>
</Header>

{#if $isLoggedIn && $currentParams.guildId}
    <SideNav bind:isOpen={isSideNavOpen} rail>
        <SideNavItems>
            <SideNavLink icon={Fade16} text="Link 1" href="/" isSelected />
            <SideNavLink icon={Fade16} text="Link 2" href="/" />
            <SideNavLink icon={Fade16} text="Link 3" href="/" />
            <SideNavMenu icon={Fade16} text="Menu">
                <SideNavMenuItem href="/" text="Link 1" />
                <SideNavMenuItem href="/" text="Link 2" />
                <SideNavMenuItem href="/" text="Link 3" />
            </SideNavMenu>
            <SideNavDivider />
            <SideNavLink icon={Fade16} text="Link 4" href="/" />
        </SideNavItems>
    </SideNav>
{/if}

<Content
    style="min-height: 100vh; padding-top: 5rem; margin-top: unset; display: flex; flex-direction: column"
>
    <slot />
</Content>
