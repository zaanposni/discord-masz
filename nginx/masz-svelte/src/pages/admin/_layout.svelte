<script lang="ts">
    import { goto } from "@roxi/routify";
    import { authUser } from "../../stores/auth";
    import { navConfig } from "../../stores/nav";
    import { Api, Dashboard, SettingsServices } from "carbon-icons-svelte";

    let checkForAdmin = true;
    $: if ($authUser && checkForAdmin) {
        if (!$authUser?.isAdmin) {
            $goto("/");
        } else {
            checkForAdmin = false;
        }
    }

    navConfig.set({
        enabled: true,
        items: [
            {
                titleKey: "admin.dashboard",
                href: "/admin",
                checkSelected: "/admin/index",
                icon: Dashboard
            },
            {
                titleKey: "admin.settings",
                href: "/admin/settings",
                icon: SettingsServices
            },
            {
                titleKey: "admin.tokens",
                href: "/admin/tokens",
                icon: Api
            }
        ]
    });
</script>

<slot />
