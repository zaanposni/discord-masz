import { derived, writable } from "svelte/store";
import type { Writable, Readable } from "svelte/store";
import type { IAuthUser } from "../models/IAuthUser";

export const authUser: Writable<IAuthUser> = writable<IAuthUser>(null);
export const isLoggedIn: Readable<boolean> = derived(authUser, currentUser => currentUser !== null);
