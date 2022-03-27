<script lang="ts">
    import NavItem from "./../components/nav/NavItem.svelte";
    import { goto, isActive, url } from "@roxi/routify";
    import { shortcut } from "../utils/shortcut.js";
    import {
        Header,
        SideNav,
        SideNavItems,
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
    import SettingsAdjust20 from "carbon-icons-svelte/lib/SettingsAdjust20";
    import { showUserSettings } from "../components/nav/store";
    import Usersettings from "../components/nav/usersettings.svelte";
    import { APP_NAME, APP_VERSION } from "../config";
    import { anyGuilds, authUser, isLoggedIn } from "../stores/auth";
    import { currentParams } from "../stores/currentParams";
    import Star24 from "carbon-icons-svelte/lib/Star24";
    import StarFilled24 from "carbon-icons-svelte/lib/StarFilled24";
    import Logout24 from "carbon-icons-svelte/lib/Logout24";
    import { favoriteGuild } from "../stores/favoriteGuild";
    import { _ } from "svelte-i18n";
    import { navConfig } from "../stores/nav";
    import deleteCookie from "../services/cookie/deleteCookie";
    import API from "../services/api/api";
    import Launch20 from "carbon-icons-svelte/lib/Launch20";
    import { showCredits } from "../components/nav/credits/store";
    import Credits from "../components/nav/credits/Credits.svelte";
    import Help20 from "carbon-icons-svelte/lib/Help20";
    import CurrentUserIcon from "../components/discord/CurrentUserIcon.svelte";
    import { applicationInfo, privacyPolicyUrl, termsOfServiceUrl } from "../stores/applicationInfo";

    if (window.location.pathname !== "/login" && !$isLoggedIn) {
        $goto("/login", { returnUrl: window.location.pathname });
    }

    let isSideNavOpen = false;
    let userIsOpen = false;
    let switcherIsOpen = false;

    function toggleFavorite(guildId: string) {
        favoriteGuild.set($favoriteGuild === guildId ? "" : guildId);
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
                  return item.text.toLowerCase().includes(lowerCaseValue) || item.description.includes(lowerCaseValue);
              })
            : [];

    function activateSearch() {
        if ($isLoggedIn) {
            searchActive = true;
        }
    }

    function logout() {
        deleteCookie("masz_access_token");
        authUser.set(null);
        $goto("/login", { returnUrl: window.location.pathname });
    }

    console.log("Cleared cache from previous session: " + API.clearCache());
</script>

<Usersettings />
<Credits />

<div on:click={activateSearch} use:shortcut={{ control: true, code: "KeyK" }} />
<Header
    company={$applicationInfo?.name ?? APP_NAME}
    platformName={APP_VERSION}
    bind:isSideNavOpen
    on:click={() => {
        $isLoggedIn ? $goto("/guilds") : undefined;
    }}>
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
                }} />
        {/if}
        <HeaderGlobalAction
            aria-label="Credits"
            icon={Help20}
            on:click={() => {
                showCredits.set(true);
            }} />
        <HeaderGlobalAction
            aria-label="Settings"
            icon={SettingsAdjust20}
            on:click={() => {
                showUserSettings.set(true);
            }} />
        {#if $isLoggedIn}
            <HeaderAction bind:isOpen={userIsOpen} icon={CurrentUserIcon}>
                <HeaderPanelLink href={$termsOfServiceUrl} target="_blank">
                    <div class="flex flex-row flex-nowrap">
                        {$_("nav.terms")}
                        <Launch20 class="ml-1" />
                    </div>
                </HeaderPanelLink>
                <HeaderPanelLink href={$privacyPolicyUrl} target="_blank">
                    <div class="flex flex-row flex-nowrap">
                        {$_("nav.privacy")}
                        <Launch20 class="ml-1" />
                    </div>
                </HeaderPanelLink>
                <HeaderPanelDivider />
                <HeaderPanelLinks>
                    <NavItem item={Logout24} text={$_("nav.logout")} on:click={logout} />
                </HeaderPanelLinks>
            </HeaderAction>
            <HeaderAction bind:isOpen={switcherIsOpen}>
                <HeaderPanelLinks>
                    {#if $authUser.isAdmin}
                        <HeaderPanelLink href={$url("/admin")}>{$_("nav.admin.base")}</HeaderPanelLink>
                        <HeaderPanelDivider />
                    {/if}
                    <HeaderPanelLink href={$url("/patchnotes")}>{$_("nav.patchnotes")}</HeaderPanelLink>
                    <HeaderPanelLink
                        on:click={() => {
                            showCredits.set(true);
                        }}>
                        {$_("nav.credits")}
                    </HeaderPanelLink>
                    <HeaderPanelLink href="https://discord.gg/5zjpzw6h3S" target="_blank">
                        <div class="flex flex-row flex-nowrap">
                            {$_("nav.community")}
                            <Launch20 class="ml-1" />
                        </div>
                    </HeaderPanelLink>
                    <HeaderPanelLink href={$termsOfServiceUrl} target="_blank">
                        <div class="flex flex-row flex-nowrap">
                            {$_("nav.terms")}
                            <Launch20 class="ml-1" />
                        </div>
                    </HeaderPanelLink>
                    <HeaderPanelLink href={$privacyPolicyUrl} target="_blank">
                        <div class="flex flex-row flex-nowrap">
                            {$_("nav.privacy")}
                            <Launch20 class="ml-1" />
                        </div>
                    </HeaderPanelLink>
                    <HeaderPanelDivider />
                    <HeaderPanelLink href={$url("/guilds")}>{$_("nav.allguilds")}</HeaderPanelLink>
                    {#if $anyGuilds}
                        <HeaderPanelDivider />
                        {#each $authUser.adminGuilds as guild (guild.id)}
                            <NavItem
                                item={$favoriteGuild === guild.id ? StarFilled24 : Star24}
                                text={guild.name}
                                on:iconClick={(e) => {
                                    toggleFavorite(guild.id);
                                    e.stopPropagation();
                                }}
                                on:click={() => {
                                    $goto(`/guilds/${guild.id}`);
                                }} />
                        {/each}
                        {#each $authUser.modGuilds as guild (guild.id)}
                            <NavItem
                                item={$favoriteGuild === guild.id ? StarFilled24 : Star24}
                                text={guild.name}
                                on:iconClick={(e) => {
                                    toggleFavorite(guild.id);
                                    e.stopPropagation();
                                }}
                                on:click={() => {
                                    $goto(`/guilds/${guild.id}`);
                                }} />
                        {/each}
                        {#each $authUser.memberGuilds as guild (guild.id)}
                            <NavItem
                                item={$favoriteGuild === guild.id ? StarFilled24 : Star24}
                                text={guild.name}
                                on:iconClick={(e) => {
                                    toggleFavorite(guild.id);
                                    e.stopPropagation();
                                }}
                                on:click={() => {
                                    $goto(`/guilds/${guild.id}/cases`);
                                }} />
                        {/each}
                        {#each $authUser.bannedGuilds as guild (guild.id)}
                            <NavItem
                                item={$favoriteGuild === guild.id ? StarFilled24 : Star24}
                                text={guild.name}
                                on:iconClick={(e) => {
                                    toggleFavorite(guild.id);
                                    e.stopPropagation();
                                }}
                                on:click={() => {
                                    $goto(`/guilds/${guild.id}/cases`);
                                }} />
                        {/each}
                    {/if}
                </HeaderPanelLinks>
            </HeaderAction>
        {/if}
    </HeaderUtilities>
</Header>

{#if $isLoggedIn && $navConfig.enabled}
    <SideNav bind:isOpen={isSideNavOpen} rail>
        <SideNavItems>
            {#each $navConfig.items as item (item.titleKey)}
                {#if item?.isAllowedToView ? item.isAllowedToView($authUser, $currentParams) : true}
                    {#if item.isDivider}
                        <SideNavDivider />
                    {:else if item.href}
                        <SideNavLink
                            icon={item.icon}
                            href={$url(item.href)}
                            text={$_("nav." + item.titleKey)}
                            isSelected={$isActive(item?.checkSelected ?? item.href, {}, { strict: true })} />
                    {:else}
                        <SideNavLink
                            class="cursor-pointer"
                            icon={item.icon}
                            on:click={item.onClick}
                            text={$_("nav." + item.titleKey)}
                            isSelected={$isActive(item?.checkSelected ?? item.href, {}, { strict: true })} />
                    {/if}
                {/if}
            {/each}
        </SideNavItems>
    </SideNav>
{/if}

<Content style="min-height: 100vh; padding-top: 5rem; margin-top: unset; display: flex; flex-direction: column">
    <slot />
</Content>
