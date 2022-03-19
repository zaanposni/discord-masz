<script lang="ts">
	import { authUser } from './../stores/auth';
    import { Button } from "carbon-components-svelte";
    import { onDestroy, onMount } from 'svelte';
    import ArrowRight32 from "carbon-icons-svelte/lib/ArrowRight32";
    import { Link } from "carbon-components-svelte";
    import Cookies from "../services/cookie";
    import API from '../services/api/api';
import type { IAuthUser } from '../models/IAuthUser';
import { toastError } from '../services/toast/store';


    let loggingIn: boolean = false;

    function login() {
        loggingIn = true;
        window.location.href="/api/v1/login";
        //authUser.set({} as any);
    }

    onMount(() => {
        document.getElementsByTagName('main')[0].classList.add('login-gradient');
    });

    onDestroy(() => {
        document.getElementsByTagName("main")[0].classList.remove("login-gradient");
    });

    function testLogin() {
        // check if cookie is set
        if (Cookies.getCookie("masz_access_token") != null) {
            loggingIn = true;
            API.get("discord/users/@me").then((res: IAuthUser) => {
                authUser.set(res);
            }).catch((err) => {
                loggingIn = false;
                toastError("You have been logged out.", "Unauthorized");
                Cookies.deleteCookie("masz_access_token");
            });
        }
    }
    testLogin();
</script>

<div class="flex flex-col justify-center items-center w-full h-full">
    <div class="flex flex-col">
        <div class="flex flex-col pt-4" style="background: var(--cds-ui-02, #ffffff)">
            <!-- add headline with ibm typography -->
            <h2 class="px-4">Login</h2>
            <div style="height: var(--cds-spacing-04)" />
            <span class="px-4">Don't have an account? <Link href="https://discord.com/register">Create a Discord account</Link></span>
            <div style="height: var(--cds-spacing-11)" />
            <div class="flex flex-row">
                <div class="grow" />
                <Button skeleton={loggingIn} class="grow" icon={ArrowRight32} on:click={login}>Login</Button>
            </div>
        </div>
        <div style="height: var(--cds-spacing-02)" />
        <div style="color: white">
            <Link href="https://discord.com/terms">Terms and Guidelines</Link>
        </div>
    </div>
</div>
