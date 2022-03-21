<script lang="ts">
    import { goto } from "@roxi/routify";
    import { authUser } from "../../stores/auth";
    import { navConfig } from "../../stores/nav";
    import { Api16, Dashboard16, SettingsServices16 } from "carbon-icons-svelte";

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
                icon: Dashboard16
            },
            {
                titleKey: "admin.settings",
                href: "/admin/settings",
                icon: SettingsServices16
            },
            {
                titleKey: "admin.tokens",
                href: "/admin/tokens",
                icon: Api16
            }
        ]
    });
</script>

<slot />
