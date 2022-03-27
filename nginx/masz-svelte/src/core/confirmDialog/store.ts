import type { Writable } from "svelte/store";
import { writable } from "svelte/store";

export const showConfirmDialog: Writable<boolean> = writable(false);
export const confirmDialogMessageKey: Writable<string> = writable("dialog.confirm.message");
export const confirmDialogReturnFunction: Writable<(confirmed: boolean) => void> = writable((confirmed) => {});
