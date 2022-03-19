<script lang="ts">
  import { goto, layout } from "@roxi/routify";
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
  } from "carbon-components-svelte";
  import Fade16 from "carbon-icons-svelte/lib/Fade16";
  import SettingsAdjust20 from "carbon-icons-svelte/lib/SettingsAdjust20";
  import UserIcon from "../components/discord/UserIcon.svelte";
  import { showUserSettings } from "../components/nav/store";

  import Usersettings from "../components/nav/usersettings.svelte";
  import { APP_NAME, APP_VERSION } from "../config";
  import { isLoggedIn } from "../stores/auth";

  $: if ($layout.path !== "/login" && !$isLoggedIn) {
    $goto("/login");
  } else {
    $goto("/dashboard");
  }

  let isSideNavOpen = false;
  let isOpen1 = false;
  let isOpen2 = false;
</script>

<Usersettings />

<Header company={APP_NAME} platformName={APP_VERSION} bind:isSideNavOpen>
  <svelte:fragment slot="skip-to-content">
    <SkipToContent />
  </svelte:fragment>
  <HeaderUtilities>
    <HeaderGlobalAction
      aria-label="Settings"
      icon={SettingsAdjust20}
      on:click={() => {
        showUserSettings.set(true);
      }}
    />
    {#if $isLoggedIn}
      <HeaderAction
        bind:isOpen={isOpen1}
        icon={UserIcon}
      >
        <HeaderPanelLinks>
          <HeaderPanelLink>Switcher item 1</HeaderPanelLink>
        </HeaderPanelLinks>
      </HeaderAction>
      <HeaderAction bind:isOpen={isOpen2}>
        <HeaderPanelLinks>
          <HeaderPanelLink>Switcher item 1</HeaderPanelLink>
        </HeaderPanelLinks>
      </HeaderAction>
    {/if}
  </HeaderUtilities>
</Header>

{#if $isLoggedIn}
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

<Content style="height: calc(100vh - 5rem); padding-top: 5rem; margin-top: unset" >
  <slot />
</Content>
