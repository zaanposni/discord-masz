<script lang="ts">
    import {
        Catalog,
        Chat,
        ChatOperational,
        Dashboard,
        Flash,
        Group,
        List,
        Police,
        ScalesTipped,
        Settings,
        TextFootnote,
        UserIdentification,
        Box,
    } from "carbon-icons-svelte";
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
                    icon: Dashboard,
                    isAllowedToView: (user, params) =>
                        user &&
                        params?.guildId &&
                        (user.isAdmin ||
                            user.adminGuilds.map((x) => x.id).includes(params.guildId) ||
                            user.modGuilds.map((x) => x.id).includes(params.guildId)),
                },
                {
                    titleKey: "placeholder00",
                    isDivider: true,
                    isAllowedToView: (user, params) =>
                        user &&
                        params?.guildId &&
                        (user.isAdmin ||
                            user.adminGuilds.map((x) => x.id).includes(params.guildId) ||
                            user.modGuilds.map((x) => x.id).includes(params.guildId)),
                },
                {
                    titleKey: "guild.cases",
                    href: `/guilds/${$currentParams.guildId}/cases`,
                    icon: List,
                },
                {
                    titleKey: "guild.evidence",
                    href: `/guilds/${$currentParams.guildId}/evidence`,
                    icon: Box,
                },
                {
                    titleKey: "guild.automods",
                    href: `/guilds/${$currentParams.guildId}/automods`,
                    icon: Flash,
                },
                {
                    titleKey: "guild.appeals",
                    href: `/guilds/${$currentParams.guildId}/appeals`,
                    icon: ScalesTipped,
                },
                {
                    titleKey: "placeholder01",
                    isDivider: true,
                    isAllowedToView: (user, params) =>
                        user &&
                        params?.guildId &&
                        (user.isAdmin ||
                            user.adminGuilds.map((x) => x.id).includes(params.guildId) ||
                            user.modGuilds.map((x) => x.id).includes(params.guildId)),
                },
                {
                    titleKey: "guild.usernotes",
                    href: `/guilds/${$currentParams.guildId}/usernotes`,
                    icon: UserIdentification,
                    isAllowedToView: (user, params) =>
                        user &&
                        params?.guildId &&
                        (user.isAdmin ||
                            user.adminGuilds.map((x) => x.id).includes(params.guildId) ||
                            user.modGuilds.map((x) => x.id).includes(params.guildId)),
                },
                {
                    titleKey: "guild.usermaps",
                    href: `/guilds/${$currentParams.guildId}/usermaps`,
                    icon: Group,
                    isAllowedToView: (user, params) =>
                        user &&
                        params?.guildId &&
                        (user.isAdmin ||
                            user.adminGuilds.map((x) => x.id).includes(params.guildId) ||
                            user.modGuilds.map((x) => x.id).includes(params.guildId)),
                },
                {
                    titleKey: "guild.messages",
                    href: `/guilds/${$currentParams.guildId}/messages`,
                    icon: Chat,
                    isAllowedToView: (user, params) =>
                        user &&
                        params?.guildId &&
                        (user.isAdmin ||
                            user.adminGuilds.map((x) => x.id).includes(params.guildId) ||
                            user.modGuilds.map((x) => x.id).includes(params.guildId)),
                },
                {
                    titleKey: "placeholder02",
                    isDivider: true,
                    isAllowedToView: (user, params) =>
                        user &&
                        params?.guildId &&
                        (user.isAdmin || user.adminGuilds.map((x) => x.id).includes(params.guildId)),
                },
                {
                    titleKey: "guild.settings.motd",
                    href: `/guilds/${$currentParams.guildId}/motd`,
                    icon: ChatOperational,
                    isAllowedToView: (user, params) =>
                        user &&
                        params?.guildId &&
                        (user.isAdmin || user.adminGuilds.map((x) => x.id).includes(params.guildId)),
                },
                {
                    titleKey: "guild.settings.auditlog",
                    href: `/guilds/${$currentParams.guildId}/auditlog`,
                    icon: Catalog,
                    isAllowedToView: (user, params) =>
                        user &&
                        params?.guildId &&
                        (user.isAdmin || user.adminGuilds.map((x) => x.id).includes(params.guildId)),
                },
                {
                    titleKey: "guild.settings.automod",
                    href: `/guilds/${$currentParams.guildId}/automodconfig`,
                    icon: Police,
                    isAllowedToView: (user, params) =>
                        user &&
                        params?.guildId &&
                        (user.isAdmin || user.adminGuilds.map((x) => x.id).includes(params.guildId)),
                },
                {
                    titleKey: "guild.settings.zalgo",
                    href: `/guilds/${$currentParams.guildId}/zalgo`,
                    icon: TextFootnote,
                    isAllowedToView: (user, params) =>
                        user &&
                        params?.guildId &&
                        (user.isAdmin || user.adminGuilds.map((x) => x.id).includes(params.guildId)),
                },
                {
                    titleKey: "guild.settings.base",
                    href: `/guilds/${$currentParams.guildId}/settings`,
                    icon: Settings,
                    isAllowedToView: (user, params) =>
                        user &&
                        params?.guildId &&
                        (user.isAdmin || user.adminGuilds.map((x) => x.id).includes(params.guildId)),
                },
            ],
        });
    }
    $: $currentParams?.guildId ? setNavConfig() : undefined;
</script>

<slot />
