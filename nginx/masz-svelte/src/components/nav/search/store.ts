import type { Readable, Writable } from "svelte/store";
import { writable, derived } from "svelte/store";
import type { ISearchEntry, ISearchEntryDefinition } from "./ISearchEntry";
import { _ } from "svelte-i18n";
import fuzzysort from "fuzzysort";
import { currentParams } from "../../../stores/currentParams";
import { authUser } from "../../../stores/auth";
import type { GotoHelper } from "@roxi/routify";


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
                    text: i18n(entry.textKey),
                    description: entry.descriptionKey ? i18n(entry.descriptionKey) : null,
                    additionalSearch: entry.additionalSearchKeys ? entry.additionalSearchKeys.map((key) => i18n(key)).join(" ") : null,
                };
                newEntry.preparedText = fuzzysort.prepare(newEntry.text);
                newEntry.preparedDescription = newEntry.description ? fuzzysort.prepare(newEntry.description) : null;
                newEntry.preparedAdditionalSearch = newEntry.additionalSearch ? newEntry.additionalSearch.map(fuzzysort.prepare) : null;
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
        }

        set(
            entries.map((entry) => {
                let newEntry: any = {
                    onSelect: entry.onSelect,
                    allowedToView: entry.allowedToView,
                    href: entry.href ?? "",
                    text: i18n(entry.textKey),
                    description: entry.descriptionKey ? i18n(entry.descriptionKey) : null,
                    additionalSearch: entry.additionalSearchKeys ? entry.additionalSearchKeys.map((key) => i18n(key)).join(" ") : null,
                };
                newEntry.preparedText = fuzzysort.prepare(newEntry.text);
                newEntry.preparedDescription = newEntry.description ? fuzzysort.prepare(newEntry.description) : null;
                newEntry.preparedAdditionalSearch = newEntry.additionalSearch ? newEntry.additionalSearch.map(fuzzysort.prepare) : null;
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
                        text: i18n(entry.textKey),
                        description: entry.descriptionKey ? i18n(entry.descriptionKey) : null,
                        additionalSearch: entry.additionalSearchKeys ? entry.additionalSearchKeys.map((key) => i18n(key)).join(" ") : null,
                    };
                    newEntry.preparedText = fuzzysort.prepare(newEntry.text);
                    newEntry.preparedDescription = newEntry.description ? fuzzysort.prepare(newEntry.description) : null;
                    newEntry.preparedAdditionalSearch = newEntry.additionalSearch ? newEntry.additionalSearch.map(fuzzysort.prepare) : null;
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
            set([...adminEntries.slice(0, 5), ...guildEntries.slice(0, 10), ...staticEntries.slice(0, 8)]);
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

let remoteSearchDebounce;
export const remoteSearchResults: Writable<ISearchEntry[]> = writable([]);
searchValue.subscribe((value) => {
    if (value.length > 3) {
        if (remoteSearchDebounce) {
            clearTimeout(remoteSearchDebounce);
        }
        remoteSearchDebounce = setTimeout(() => {
            let promise = new Promise((resolve) => {
                remoteSearchDebounce = setTimeout(() => {
                    resolve([
                        {
                            text: "remote example",
                            description: "remote example",
                            onSelect: () => {
                                console.log("hi2");
                            },
                        },
                    ]);
                }, 5000);
            });
            promise.then((results) => {
                remoteSearchResults.set(results as any);
            });
        }, 200);
    } else {
        remoteSearchResults.set([]);
    }
});

export const accessibleGuildsResults: Readable<ISearchEntry[]> = derived(
    [authUser, searchValue],
    ([currentUser, value], set) => {
        if (currentUser?.adminGuilds != null) {
            if (!value) {
                const guilds = currentUser.adminGuilds
                    .concat(currentUser.modGuilds)
                    .concat(currentUser.memberGuilds)
                    .concat(currentUser.bannedGuilds);

                set(
                    guilds.slice(0, 5).map((g) => {
                        return {
                            text: `Guild: ${g.name}`,
                            href: `/guild/${g.id}`,
                            description: g.id,
                            onSelect: (gotoHelper: GotoHelper) => {
                                gotoHelper(`/guilds/${g.id}`);
                            },
                        };
                    })
                );

                return;
            }

            value = value.toLocaleLowerCase();

            const options = {
                limit: 5,
                allowTypo: true,
                threshold: -10000,
                keys: ["name"],
            };

            const adminResults = fuzzysort.go(value, currentUser.adminGuilds, options).map((result) => result.obj);
            const moderatorResults = fuzzysort.go(value, currentUser.modGuilds, options).map((result) => result.obj);
            const memberResults = fuzzysort.go(value, currentUser.memberGuilds, options).map((result) => result.obj);
            const bannedResults = fuzzysort.go(value, currentUser.bannedGuilds, options).map((result) => result.obj);

            const guilds = adminResults.concat(moderatorResults).concat(memberResults).concat(bannedResults);

            set(
                guilds.slice(0, 5).map((g) => {
                    return {
                        text: `Guild: ${g.name}`,
                        href: `/guild/${g.id}`,
                        description: g.id,
                        onSelect: (gotoHelper: GotoHelper) => {
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
    [allStaticSearchResults, accessibleGuildsResults, remoteSearchResults, authUser],
    ([staticResults, guildsResults, remoteResults, currentUser]) => {
        const filtered = staticResults.filter((x) => (x?.allowedToView ? x.allowedToView(currentUser) : true));

        return remoteResults.concat(guildsResults).concat(filtered).slice(0, 30);
    }
);

showSearch.subscribe((value) => {
    // correctly reset search results
    // also reset on search open to erase search results that came in async after the last close
    searchValue.set("");
    remoteSearchResults.set([]);
});

// INIT

adminStaticSearchEntries.set([
    {
        textKey: "admin",
        descriptionKey: "admin.",
        onSelect: () => {
            console.log("admin");
        },
    },
]);

staticSearchEntries.set([
    {
        textKey: "Kubernetes Service",
        descriptionKey: "Deploy secure.",
        onSelect: () => {
            console.log("hi");
        },
        allowedToView: () => {
            return false;
        },
    },
    {
        textKey: "google Service",
        descriptionKey: "Deploy secure.",
        onSelect: () => {
            console.log("hi");
        },
    },
    {
        textKey: "facebook Service",
        descriptionKey: "Deploy secure.",
        onSelect: () => {
            console.log("hi");
        },
    },
]);
