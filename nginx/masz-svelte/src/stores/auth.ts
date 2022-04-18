import { derived, writable } from "svelte/store";
import type { Writable, Readable } from "svelte/store";
import type { IAuthUser } from "../models/IAuthUser";

export const authUser: Writable<IAuthUser> = writable<IAuthUser>(null);
export const isLoggedIn: Readable<boolean> = derived(authUser, (currentUser) => currentUser !== null);
export const anyGuilds: Readable<boolean> = derived(
    authUser,
    (currentUser) => {
        return (
            currentUser?.adminGuilds?.length !== 0 ||
            currentUser?.modGuilds?.length !== 0 ||
            currentUser?.memberGuilds?.length !== 0 ||
            currentUser?.bannedGuilds?.length !== 0
        );
    },
    false
);

export function isModeratorInGuild(user: IAuthUser, guildId: string): boolean {
    return user?.isAdmin || user?.adminGuilds?.find(x => x.id === guildId) !== undefined || user?.modGuilds?.find(x => x.id === guildId) !== undefined;
}

export function isAdminInGuild(user: IAuthUser, guildId: string): boolean {
    return user?.isAdmin || user?.adminGuilds?.find(x => x.id === guildId) !== undefined;
}
