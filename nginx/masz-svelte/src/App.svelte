<script lang="ts">
  import { Router } from "@roxi/routify";
  import { routes } from "../.routify/routes";
  import "carbon-components-svelte/css/all.css";

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
  import UserAvatarFilledAlt20 from "carbon-icons-svelte/lib/UserAvatarFilledAlt20";

  import Theme from "./components/nav/theme.svelte";

  let isSideNavOpen = false;
  let isOpen1 = false;
  let isOpen2 = false;
</script>

<Header company="MASZ" platformName="v3.0" bind:isSideNavOpen>
  <svelte:fragment slot="skip-to-content">
    <SkipToContent />
  </svelte:fragment>
  <HeaderUtilities>
    <HeaderGlobalAction aria-label="Settings" icon={SettingsAdjust20} />
    <HeaderAction bind:isOpen={isOpen1} icon={UserAvatarFilledAlt20} closeIcon={UserAvatarFilledAlt20}>
      <HeaderPanelLinks>
        <HeaderPanelDivider>Switcher subject 1</HeaderPanelDivider>
        <HeaderPanelLink>Switcher item 1</HeaderPanelLink>
        <HeaderPanelLink>Switcher item 2</HeaderPanelLink>
        <HeaderPanelLink>Switcher item 3</HeaderPanelLink>
        <HeaderPanelLink>Switcher item 4</HeaderPanelLink>
        <HeaderPanelDivider>Switcher subject 2</HeaderPanelDivider>
        <HeaderPanelLink>Switcher item 1</HeaderPanelLink>
        <HeaderPanelLink>Switcher item 2</HeaderPanelLink>
        <HeaderPanelDivider>Switcher subject 3</HeaderPanelDivider>
        <HeaderPanelLink>Switcher item 1</HeaderPanelLink>
      </HeaderPanelLinks>
    </HeaderAction>
    <HeaderAction bind:isOpen={isOpen2}>
      <HeaderPanelLinks>
        <HeaderPanelDivider>Switcher subject 1</HeaderPanelDivider>
        <HeaderPanelLink>Switcher item 1</HeaderPanelLink>
        <HeaderPanelDivider>Switcher subject 2</HeaderPanelDivider>
        <HeaderPanelLink>Switcher item 1</HeaderPanelLink>
        <HeaderPanelLink>Switcher item 2</HeaderPanelLink>
        <HeaderPanelLink>Switcher item 3</HeaderPanelLink>
        <HeaderPanelLink>Switcher item 4</HeaderPanelLink>
        <HeaderPanelLink>Switcher item 5</HeaderPanelLink>
      </HeaderPanelLinks>
    </HeaderAction>
  </HeaderUtilities>
</Header>

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

<Content>
  <main>
    <Theme />
    <Router {routes} />
  </main>
</Content>

<style global lang="postcss">
  @tailwind base;
  @tailwind components;
  @tailwind utilities;
</style>
