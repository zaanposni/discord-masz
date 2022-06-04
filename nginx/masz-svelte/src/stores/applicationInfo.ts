import { derived, Readable, Writable } from "svelte/store";
import { writable } from "svelte/store";
import type { IDiscordApplication } from "../models/IDiscordApplication";

export const applicationInfo: Writable<IDiscordApplication> = writable(null);
export const termsOfServiceUrl: Readable<string> = derived(applicationInfo, (info) => {
    return info?.termsOfServiceUrl ?? "legal.html";
}, "legal.html");
export const privacyPolicyUrl: Readable<string> = derived(applicationInfo, (info) => {
    return info?.privacyPolicyUrl ?? "privacy.html";
}, "privacy.html");

applicationInfo.subscribe((data) => {
    let element = document.getElementById("tabtitle");
    if (element) {
        element.innerText = data?.name ?? "MASZ";
    }
    element = document.getElementById("tabicon");
    if (element && data?.iconUrl?.length) {
        element.setAttribute("href", data.iconUrl)
    }
})
