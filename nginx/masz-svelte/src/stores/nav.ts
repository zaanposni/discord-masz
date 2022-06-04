import { writable } from "svelte/store";
import type { Writable } from "svelte/store";
import type { INavConfig } from "../models/INavConfig";

export const navConfig: Writable<INavConfig> = writable({ enabled: false, items: [] });
