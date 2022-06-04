<script lang="ts">
    import { goto, isActive, params } from "@roxi/routify";
    import { authUser } from "./../stores/auth";
    import { Button, OutboundLink } from "carbon-components-svelte";
    import { onDestroy, onMount } from "svelte";
    import ArrowRight32 from "carbon-icons-svelte/lib/ArrowRight32";
    import Cookies from "../services/cookie";
    import API from "../services/api/api";
    import type { IAuthUser } from "../models/IAuthUser";
    import { toastError } from "../services/toast/store";
    import { navConfig } from "../stores/nav";
    import { termsOfServiceUrl, privacyPolicyUrl } from "../stores/applicationInfo";
    import { _, isLoading } from "svelte-i18n";

    let loggingIn: boolean = false;

    function login() {
        loggingIn = true;
        window.location.href = "/api/v1/login";
    }

    onMount(() => {
        navConfig.set({
            enabled: false,
            items: [],
        });
        document.getElementsByTagName("main")[0].classList.add("login-gradient");
    });

    onDestroy(() => {
        document.getElementsByTagName("main")[0].classList.remove("login-gradient");
    });

    function testLogin() {
        if (Cookies.getCookie("masz_access_token") != null) {
            loggingIn = true;
            API.get("discord/users/@me")
                .then((res: IAuthUser) => {
                    authUser.set(res);
                    removeKofi();
                    if ($params.returnUrl && $params.returnUrl !== "/") {
                        $goto($params.returnUrl);
                    } else {
                        $goto("/guilds");
                    }
                })
                .catch((err) => {
                    loggingIn = false;
                    toastError("You have been logged out.", "Unauthorized");
                    Cookies.deleteCookie("masz_access_token");
                });
        }
    }
    testLogin();

    const interval = setInterval(() => {
        if (kofiWidgetOverlay && !$isLoading && $isActive("/login")) {
            clearInterval(interval);
            drawKofi();
        }
    }, 200);

    onDestroy(() => {
        setTimeout(removeKofi, 500);
    });

    function drawKofi() {
        kofiWidgetOverlay.draw("zaanposni", {
            type: "floating-chat",
            "floating-chat.donateButton.text": $_("supportme"),
            "floating-chat.donateButton.background-color": "#00b9fe",
            "floating-chat.donateButton.text-color": "#fff",
        });
    }

    function removeKofi() {
        clearInterval(interval);
        const containers = document.getElementsByClassName("floatingchat-container-wrap");
        if (containers.length > 0) {
            containers[0].parentElement.remove();
        }
    }
</script>

<style>
    :global(.floatingchat-container-wrap) {
        max-width: 250px !important;
    }

    :global(.floatingchat-container-wrap > iframe) {
        width: 250px !important;
    }
</style>

<div class="flex flex-col justify-center items-center grow">
    <div class="flex flex-col">
        <div class="flex flex-col pt-4" style="background: var(--cds-ui-02, #ffffff)">
            <!-- add headline with ibm typography -->
            <h2 class="px-4">{$_("login.title")}</h2>
            <div style="height: var(--cds-spacing-04)" />
            <span class="px-4"
                >{$_("login.noaccount")} <OutboundLink href="https://discord.com/register">{$_("login.createaccount")}</OutboundLink></span>
            <div style="height: var(--cds-spacing-11)" />
            <div class="flex flex-row">
                <div class="grow" />
                <Button skeleton={loggingIn} class="grow" icon={ArrowRight32} on:click={login}>{$_("login.title")}</Button>
            </div>
        </div>
        <div style="height: var(--cds-spacing-02)" />
        <div style="color: white">
            <OutboundLink inline href={$termsOfServiceUrl}>{$_("login.terms")}</OutboundLink>
            {$_("login.and")}
            <OutboundLink inline href={$privacyPolicyUrl}>{$_("login.privacy")}</OutboundLink>
        </div>
    </div>
</div>
