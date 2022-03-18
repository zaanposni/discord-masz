import {writable} from "svelte/store";
import type { Writable } from "svelte/store";

export const showUserSettings: Writable<boolean> = writable(false);
