<script lang="ts">
    import { Catalog16, Chat16, ChatOperational16, Dashboard16, Flash16, Group16, List16, Police16, ScalesTipped16, Settings16, TextFootnote16, UserIdentification16 } from "carbon-icons-svelte";
    import { currentParams } from "../../../stores/currentParams";
    import { navConfig } from "../../../stores/nav";

    function setNavConfig() {
        navConfig.set({
            enabled: true,
            items: [
                {
                    titleKey: "guild.dashboard",
                    href: "/guilds/" + $currentParams.guildId,
                    checkSelected: "/guilds/" + $currentParams.guildId + "/index",
                    icon: Dashboard16,
                    isAllowedToView: (user, params) => user && params?.guildId && (user.isAdmin || user.adminGuilds.map(x => x.id).includes(params.guildId) || user.modGuilds.map(x => x.id).includes(params.guildId))
                },
                {
                    titleKey: 'placeholder00',
                    isDivider: true,
                    isAllowedToView: (user, params) => user && params?.guildId && (user.isAdmin || user.adminGuilds.map(x => x.id).includes(params.guildId) || user.modGuilds.map(x => x.id).includes(params.guildId))
                },
                {
                    titleKey: "guild.cases",
                    href: `/guilds/${$currentParams.guildId}/cases`,
                    icon: List16,
                },
                {
                    titleKey: "guild.automods",
                    href: `/guilds/${$currentParams.guildId}/automods`,
                    icon: Flash16,
                },
                {
                    titleKey: "guild.appeals",
                    href: `/guilds/${$currentParams.guildId}/appeals`,
                    icon: ScalesTipped16,
                },
                {
                    titleKey: 'placeholder01',
                    isDivider: true,
                    isAllowedToView: (user, params) => user && params?.guildId && (user.isAdmin || user.adminGuilds.map(x => x.id).includes(params.guildId) || user.modGuilds.map(x => x.id).includes(params.guildId))
                },
                {
                    titleKey: "guild.usernotes",
                    href: `/guilds/${$currentParams.guildId}/usernotes`,
                    icon: UserIdentification16,
                    isAllowedToView: (user, params) => user && params?.guildId && (user.isAdmin || user.adminGuilds.map(x => x.id).includes(params.guildId) || user.modGuilds.map(x => x.id).includes(params.guildId))
                },
                {
                    titleKey: "guild.usermaps",
                    href: `/guilds/${$currentParams.guildId}/usermaps`,
                    icon: Group16,
                    isAllowedToView: (user, params) => user && params?.guildId && (user.isAdmin || user.adminGuilds.map(x => x.id).includes(params.guildId) || user.modGuilds.map(x => x.id).includes(params.guildId))
                },
                {
                    titleKey: "guild.messages",
                    href: `/guilds/${$currentParams.guildId}/messages`,
                    icon: Chat16,
                    isAllowedToView: (user, params) => user && params?.guildId && (user.isAdmin || user.adminGuilds.map(x => x.id).includes(params.guildId) || user.modGuilds.map(x => x.id).includes(params.guildId))
                },
                {
                    titleKey: 'placeholder02',
                    isDivider: true,
                    isAllowedToView: (user, params) => user && params?.guildId && (user.isAdmin || user.adminGuilds.map(x => x.id).includes(params.guildId))
                },
                {
                    titleKey: "guild.settings.motd",
                    href: `/guilds/${$currentParams.guildId}/motd`,
                    icon: ChatOperational16,
                    isAllowedToView: (user, params) => user && params?.guildId && (user.isAdmin || user.adminGuilds.map(x => x.id).includes(params.guildId))
                },
                {
                    titleKey: "guild.settings.auditlog",
                    href: `/guilds/${$currentParams.guildId}/auditlog`,
                    icon: Catalog16,
                    isAllowedToView: (user, params) => user && params?.guildId && (user.isAdmin || user.adminGuilds.map(x => x.id).includes(params.guildId))
                },
                {
                    titleKey: "guild.settings.automod",
                    href: `/guilds/${$currentParams.guildId}/automodconfig`,
                    icon: Police16,
                    isAllowedToView: (user, params) => user && params?.guildId && (user.isAdmin || user.adminGuilds.map(x => x.id).includes(params.guildId))
                },
                {
                    titleKey: "guild.settings.zalgo",
                    href: `/guilds/${$currentParams.guildId}/zalgo`,
                    icon: TextFootnote16,
                    isAllowedToView: (user, params) => user && params?.guildId && (user.isAdmin || user.adminGuilds.map(x => x.id).includes(params.guildId))
                },
                {
                    titleKey: "guild.settings.base",
                    href: `/guilds/${$currentParams.guildId}/settings`,
                    icon: Settings16,
                    isAllowedToView: (user, params) => user && params?.guildId && (user.isAdmin || user.adminGuilds.map(x => x.id).includes(params.guildId))
                },
            ],
        });
    }
    $: $currentParams?.guildId ? setNavConfig() : undefined;
</script>

<slot />
