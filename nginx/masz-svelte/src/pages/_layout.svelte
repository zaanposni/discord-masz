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
    import MediaQuery from "../core/MediaQuery.svelte";
    import ConfirmDialog from "../core/confirmDialog/ConfirmDialog.svelte";
    import { searchValue, showSearch, searchResults } from "../components/nav/search/store";
    import GuildIcon from "../components/discord/GuildIcon.svelte";
    import { fade } from "svelte/transition";

    if (window.location.pathname !== "/login" && !$isLoggedIn) {
        $goto("/login", { returnUrl: window.location.pathname });
    }

    let isSideNavOpen = false;
    let userIsOpen = false;
    let switcherIsOpen = false;

    function toggleFavorite(guildId: string) {
        favoriteGuild.set($favoriteGuild === guildId ? "" : guildId);
    }

    function onSearchSelect(e: { detail: { selectedResult: any } }) {
        e.detail.selectedResult.onSelect($goto, $currentParams);
    }

    function activateSearch() {
        if ($isLoggedIn) {
            showSearch.set(true);
        }
    }

    function logout() {
        deleteCookie("masz_access_token");
        $goto("/login", { returnUrl: window.location.pathname });
        setTimeout(() => {
            authUser.set(null);
            console.log("Cleared cache: " + API.clearCache());
        }, 200);
    }

    console.log("Cleared cache from previous session: " + API.clearCache());
</script>

<Usersettings />
<Credits />
<ConfirmDialog />

<div on:click={activateSearch} use:shortcut={{ control: true, code: "KeyK" }} />

<MediaQuery query="(min-width: 1000px)" let:matches>
    <Header
        company={$applicationInfo?.name ?? APP_NAME}
        platformName={matches ? APP_VERSION : ""}
        bind:isSideNavOpen
        on:click={() => {
            $isLoggedIn ? $goto("/guilds") : undefined;
        }}>
        <svelte:fragment slot="skip-to-content">
            <SkipToContent />
        </svelte:fragment>
        <div slot="platform" class="flex flex-row items-center">
            {#if matches}
                {APP_VERSION}
                {#if $currentParams?.guildId && $currentParams?.guild}
                    <div class="flex flex-row items-center" transition:fade|local>
                        <span class="font-normal mx-2">|</span>
                        {#key $currentParams.guild}
                            <GuildIcon class="max-h-[1.5rem] max-w-[1.5rem]" guild={$currentParams.guild} />
                        {/key}
                        <span class="font-normal ml-2">{$currentParams.guild.name}</span>
                    </div>
                {/if}
            {/if}
        </div>
        <HeaderUtilities>
            {#if matches}
                {#if $isLoggedIn}
                    <HeaderSearch
                        bind:value={$searchValue}
                        bind:active={$showSearch}
                        placeholder="Search services"
                        results={$searchResults}
                        on:select={onSearchSelect} />
                {/if}
                <HeaderGlobalAction
                    aria-label="Credits"
                    icon={Help20}
                    on:click={() => {
                        showCredits.set(true);
                    }} />
            {/if}
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
                        <HeaderPanelLink href="https://github.com/zaanposni/discord-masz/issues/new/choose" target="_blank">
                            <div class="flex flex-row flex-nowrap">
                                {$_("nav.reportabug")}
                                <Launch20 class="ml-1" />
                            </div>
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
</MediaQuery>

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
