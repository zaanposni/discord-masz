import { writable } from "svelte/store";
import type { Writable } from "svelte/store";
import type { IToast } from "./IToast";

export const toasts: Writable<IToast[]> = writable([]);

export const toastError = (message: string, title: string = "", timeout: number = 3000, dismissible: boolean = true) => {
    addToast({
        message,
        title,
        type: "error",
        timeout,
        dismissible,
    });
};

export const toastInfo = (message: string, title: string = "", timeout: number = 3000, dismissible: boolean = true) => {
    addToast({
        message,
        title,
        type: "info",
        timeout,
        dismissible,
    });
};

export const toastWarning = (message: string, title: string = "", timeout: number = 3000, dismissible: boolean = true) => {
    addToast({
        message,
        title,
        type: "warning",
        timeout,
        dismissible,
    });
};

export const toastSuccess = (message: string, title: string = "", timeout: number = 3000, dismissible: boolean = true) => {
    addToast({
        message,
        title,
        type: "success",
        timeout,
        dismissible,
    });
};

export const addToast = (toast: IToast) => {
    // Create a unique ID so we can easily find/remove it
    // if it is dismissible/has a timeout.
    const id = Math.floor(Math.random() * 10000);

    // Setup some sensible defaults for a toast.
    const defaults = {
        id,
        type: "info",
        dismissible: true,
        timeout: 5000,
        hovered: false,
    } as IToast;

    let updatedToast = { ...defaults, ...toast };

    if (updatedToast.title) {
        updatedToast.title = capitalizeFirstLetter(updatedToast.title);
    } else {
        updatedToast.title = updatedToast.message;
        updatedToast.message = "";
    }

    updatedToast.dismissAt = Date.now() + updatedToast.timeout;

    // Push the toast to the top of the list of toasts
    toasts.update((all) => [updatedToast, ...all]);
};

export const dismissToast = (id) => {
    toasts.update((all) => all.filter((t) => t.id !== id));
};

function capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}

setInterval(() => {
    toasts.update((all) => all.filter((t) => t.hovered || t.dismissAt > Date.now() || !t.dismissible));
}, 200);

export const setToastHovered = (id, hovered) => {
    toasts.update((all) =>
        all.map((t) => {
            if (t.id === id && t.dismissible) {
                t.hovered = hovered;
                if (!hovered && t.dismissAt + 500 < Date.now()) {
                    t.dismissAt = Date.now() + 2500;
                }
            }
            return t;
        })
    );
};
