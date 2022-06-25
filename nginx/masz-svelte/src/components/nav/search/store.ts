import type { Readable, Writable } from "svelte/store";
import { writable, derived } from "svelte/store";
import type { ISearchEntry, ISearchEntryDefinition } from "./ISearchEntry";
import { _ } from "svelte-i18n";
import fuzzysort from "fuzzysort";
import { currentParams } from "../../../stores/currentParams";
import { authUser } from "../../../stores/auth";
import type { GotoHelper } from "@roxi/routify";
import type { IRouteParams } from "../../../models/IRouteParams";
import type { IAuthUser } from "../../../models/IAuthUser";
import { showCredits } from "../credits/store";
import { showUserSettings } from "../store";

export const showSearch: Writable<boolean> = writable(false);
export const searchValue: Writable<string> = writable("");

const staticSearchEntries: Writable<ISearchEntryDefinition[]> = writable([]);
const staticsearchEntriesTranslated: Readable<ISearchEntry[]> = derived(
    [staticSearchEntries, _],
    ([entries, i18n], set) => {
        set(
            entries.map((entry) => {
                let newEntry: any = {
                    onSelect: entry.onSelect,
                    allowedToView: entry.allowedToView,
                    href: entry.href ?? "",
                    hidePerDefault: entry.hidePerDefault,
                    text: i18n(entry.textKey),
                    description: entry.descriptionKey ? i18n(entry.descriptionKey) : null,
                    additionalSearch: entry.additionalSearchKeys ? entry.additionalSearchKeys.map((key) => i18n(key)).join(" ") : null,
                };
                newEntry.preparedText = fuzzysort.prepare(newEntry.text);
                newEntry.preparedDescription = newEntry.description ? fuzzysort.prepare(newEntry.description) : null;
                newEntry.preparedAdditionalSearch = newEntry.additionalSearch ? fuzzysort.prepare(newEntry.additionalSearch) : null;
                return newEntry;
            })
        );
    },
    []
);

export const guildStaticSearchEntries: Writable<ISearchEntryDefinition[]> = writable([]);
export const guildStaticSearchEntriesTranslated: Readable<ISearchEntry[]> = derived(
    [guildStaticSearchEntries, _, currentParams],
    ([entries, i18n, currentParams], set) => {
        if (!currentParams.guild) {
            set([]);
            return;
        }

        set(
            entries.map((entry) => {
                let newEntry: any = {
                    onSelect: entry.onSelect,
                    allowedToView: entry.allowedToView,
                    href: entry.href ?? "",
                    hidePerDefault: entry.hidePerDefault,
                    text: i18n(entry.textKey),
                    description: entry.descriptionKey ? i18n(entry.descriptionKey) : null,
                    additionalSearch: entry.additionalSearchKeys ? entry.additionalSearchKeys.map((key) => i18n(key)).join(" ") : null,
                };
                newEntry.preparedText = fuzzysort.prepare(newEntry.text);
                newEntry.preparedDescription = newEntry.description ? fuzzysort.prepare(newEntry.description) : null;
                newEntry.preparedAdditionalSearch = newEntry.additionalSearch ? fuzzysort.prepare(newEntry.additionalSearch) : null;
                return newEntry;
            })
        );
    },
    []
);

export const adminStaticSearchEntries: Writable<ISearchEntryDefinition[]> = writable([]);
export const adminStaticSearchEntriesTranslated: Readable<ISearchEntry[]> = derived(
    [adminStaticSearchEntries, _, authUser],
    ([entries, i18n, currentUser], set) => {
        if (currentUser?.isAdmin === true) {
            set(
                entries.map((entry) => {
                    let newEntry: any = {
                        onSelect: entry.onSelect,
                        allowedToView: entry.allowedToView,
                        href: entry.href ?? "",
                        hidePerDefault: entry.hidePerDefault,
                        text: i18n(entry.textKey),
                        description: entry.descriptionKey ? i18n(entry.descriptionKey) : null,
                        additionalSearch: entry.additionalSearchKeys ? entry.additionalSearchKeys.map((key) => i18n(key)).join(" ") : null,
                    };
                    newEntry.preparedText = fuzzysort.prepare(newEntry.text);
                    newEntry.preparedDescription = newEntry.description ? fuzzysort.prepare(newEntry.description) : null;
                    newEntry.preparedAdditionalSearch = newEntry.additionalSearch ? fuzzysort.prepare(newEntry.additionalSearch) : null;
                    return newEntry;
                })
            );
        }
    },
    []
);

export const allStaticSearchResults: Readable<ISearchEntry[]> = derived(
    [staticsearchEntriesTranslated, guildStaticSearchEntriesTranslated, adminStaticSearchEntriesTranslated, searchValue],
    ([staticEntries, guildEntries, adminEntries, value], set) => {
        if (!value) {
            set([
                ...adminEntries.filter((x) => !x?.hidePerDefault).slice(0, 5),
                ...guildEntries.filter((x) => !x?.hidePerDefault).slice(0, 10),
                ...staticEntries.filter((x) => !x?.hidePerDefault).slice(0, 8),
            ]);
            return;
        }

        value = value.toLocaleLowerCase();

        const options = {
            allowTypo: true,
            threshold: -10000,
            keys: ["preparedText", "preparedDescription", "preparedAdditionalSearch"],
        };

        const adminResults = fuzzysort
            .go(value, adminEntries, {
                ...options,
                limit: 5,
            })
            .map((result) => result.obj);
        const guildResults = fuzzysort
            .go(value, guildEntries, {
                ...options,
                limit: 10,
            })
            .map((result) => result.obj);
        const staticResults = fuzzysort
            .go(value, staticEntries, {
                ...options,
                limit: 8,
            })
            .map((result) => result.obj);

        set(adminResults.concat(guildResults.concat(staticResults)));
    }
);

export const accessibleGuildsResults: Readable<ISearchEntry[]> = derived(
    [authUser, searchValue, _],
    ([currentUser, value, i18n], set) => {
        if (currentUser?.adminGuilds != null) {
            if (!value) {
                const guilds = currentUser.adminGuilds
                    .concat(currentUser.modGuilds)
                    .concat(currentUser.memberGuilds)
                    .concat(currentUser.bannedGuilds);

                set(
                    guilds.slice(0, 3).map((g) => {
                        return {
                            text: `${i18n("nav.guild.base")}: ${g.name}`,
                            href: `/guild/${g.id}`,
                            description: g.id,
                            onSelect: (gotoHelper: GotoHelper, currentParams: IRouteParams) => {
                                gotoHelper(`/guilds/${g.id}`);
                            },
                        };
                    })
                );

                return;
            }

            value = value.toLocaleLowerCase();

            const options = {
                limit: 3,
                allowTypo: true,
                threshold: -10000,
                keys: ["name", "id"],
            };

            const adminResults = fuzzysort.go(value, currentUser.adminGuilds, options).map((result) => result.obj);
            const moderatorResults = fuzzysort.go(value, currentUser.modGuilds, options).map((result) => result.obj);
            const memberResults = fuzzysort.go(value, currentUser.memberGuilds, options).map((result) => result.obj);
            const bannedResults = fuzzysort.go(value, currentUser.bannedGuilds, options).map((result) => result.obj);

            const guilds = adminResults.concat(moderatorResults).concat(memberResults).concat(bannedResults);

            set(
                guilds.slice(0, 3).map((g) => {
                    return {
                        text: `${i18n("nav.guild.base")}: ${g.name}`,
                        href: `/guild/${g.id}`,
                        description: g.id,
                        onSelect: (gotoHelper: GotoHelper, currentParams: IRouteParams) => {
                            gotoHelper(`/guilds/${g.id}`);
                        },
                    };
                })
            );
        }
    },
    []
);

export const searchResults: Readable<ISearchEntry[]> = derived(
    [allStaticSearchResults, accessibleGuildsResults, authUser, currentParams],
    ([staticResults, guildsResults, currentUser, params]) => {
        const filtered = staticResults.filter((x) => (x?.allowedToView ? x.allowedToView(currentUser, params) : true));

        return guildsResults.concat(filtered).slice(0, 30);
    }
);

showSearch.subscribe((value) => {
    // correctly reset search results
    searchValue.set("");
});

// INIT

guildStaticSearchEntries.set([
    {
        textKey: "nav.guild.dashboard",
        allowedToView: (user: IAuthUser, params: IRouteParams) =>
            user &&
            params?.guildId &&
            (user.isAdmin || user.adminGuilds.map((x) => x.id).includes(params.guildId) || user.modGuilds.map((x) => x.id).includes(params.guildId)),
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            gotoHelper(`/guilds/${params.guildId}`);
        },
    },
    {
        textKey: "nav.guild.cases",
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            gotoHelper(`/guilds/${params.guildId}/cases`);
        },
    },
    {
        textKey: "nav.guild.automods",
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            gotoHelper(`/guilds/${params.guildId}/automods`);
        },
    },
    {
        textKey: "nav.guild.appeals",
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            gotoHelper(`/guilds/${params.guildId}/appeals`);
        },
    },
    {
        textKey: "nav.guild.usernotes",
        allowedToView: (user: IAuthUser, params: IRouteParams) =>
            user &&
            params?.guildId &&
            (user.isAdmin || user.adminGuilds.map((x) => x.id).includes(params.guildId) || user.modGuilds.map((x) => x.id).includes(params.guildId)),
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            gotoHelper(`/guilds/${params.guildId}/usernotes`);
        },
    },
    {
        textKey: "nav.guild.usermaps",
        allowedToView: (user: IAuthUser, params: IRouteParams) =>
            user &&
            params?.guildId &&
            (user.isAdmin || user.adminGuilds.map((x) => x.id).includes(params.guildId) || user.modGuilds.map((x) => x.id).includes(params.guildId)),
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            gotoHelper(`/guilds/${params.guildId}/usermaps`);
        },
    },
    {
        textKey: "nav.guild.messages",
        additionalSearchKeys: ["schedule"],
        allowedToView: (user: IAuthUser, params: IRouteParams) =>
            user &&
            params?.guildId &&
            (user.isAdmin || user.adminGuilds.map((x) => x.id).includes(params.guildId) || user.modGuilds.map((x) => x.id).includes(params.guildId)),
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            gotoHelper(`/guilds/${params.guildId}/messages`);
        },
    },
    {
        textKey: "nav.guild.settings.motd",
        additionalSearchKeys: ["message of the day"],
        allowedToView: (user: IAuthUser, params: IRouteParams) =>
            user && params?.guildId && (user.isAdmin || user.adminGuilds.map((x) => x.id).includes(params.guildId)),
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            gotoHelper(`/guilds/${params.guildId}/motd`);
        },
    },
    {
        textKey: "nav.guild.settings.auditlog",
        allowedToView: (user: IAuthUser, params: IRouteParams) =>
            user && params?.guildId && (user.isAdmin || user.adminGuilds.map((x) => x.id).includes(params.guildId)),
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            gotoHelper(`/guilds/${params.guildId}/auditlog`);
        },
    },
    {
        textKey: "nav.guild.settings.automod",
        allowedToView: (user: IAuthUser, params: IRouteParams) =>
            user && params?.guildId && (user.isAdmin || user.adminGuilds.map((x) => x.id).includes(params.guildId)),
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            gotoHelper(`/guilds/${params.guildId}/automodconfig`);
        },
    },
    {
        textKey: "nav.guild.settings.zalgo",
        allowedToView: (user: IAuthUser, params: IRouteParams) =>
            user && params?.guildId && (user.isAdmin || user.adminGuilds.map((x) => x.id).includes(params.guildId)),
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            gotoHelper(`/guilds/${params.guildId}/zalgo`);
        },
    },
    {
        textKey: "nav.guild.settings.search",
        descriptionKey: "nav.guild.settings.searchdetails",
        allowedToView: (user: IAuthUser, params: IRouteParams) =>
            user && params?.guildId && (user.isAdmin || user.adminGuilds.map((x) => x.id).includes(params.guildId)),
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            gotoHelper(`/guilds/${params.guildId}/settings`);
        },
    },
]);

adminStaticSearchEntries.set([
    {
        textKey: "nav.admin.base",
        descriptionKey: "nav.admin.searchdescription",
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            gotoHelper(`/admin`);
        },
    },
]);

staticSearchEntries.set([
    {
        textKey: "nav.allguilds",
        descriptionKey: "",
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            gotoHelper(`/guilds`);
        },
    },
    {
        textKey: "settings.base",
        descriptionKey: "settings.basedetails",
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            showUserSettings.set(true);
        },
        hidePerDefault: true,
    },
    {
        textKey: "settings.theme",
        descriptionKey: "settings.themedetails",
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            showUserSettings.set(true);
        },
    },
    {
        textKey: "settings.language",
        descriptionKey: "settings.languagedetails",
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            showUserSettings.set(true);
        },
        hidePerDefault: true,
    },
    {
        textKey: "nav.patchnotes",
        descriptionKey: "nav.patchnotesdetails",
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            gotoHelper(`/patchnotes`);
        },
    },
    {
        textKey: "nav.credits",
        descriptionKey: "nav.creditsdetails",
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            showCredits.set(true);
        },
        hidePerDefault: true,
    },
    {
        textKey: "nav.community",
        href: "https://discord.gg/5zjpzw6h3S",
        descriptionKey: "nav.communitydetails",
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            window.open("https://discord.gg/5zjpzw6h3S", "_blank").focus();
        },
        hidePerDefault: true,
    },
    {
        textKey: "nav.reportabug",
        href: "https://github.com/zaanposni/discord-masz/issues/new/choose",
        descriptionKey: "nav.reportabugdetails",
        onSelect: (gotoHelper: GotoHelper, params: IRouteParams) => {
            window.open("https://github.com/zaanposni/discord-masz/issues/new/choose", "_blank").focus();
        },
        hidePerDefault: true,
    },
]);
