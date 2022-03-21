<script lang="ts">
    import { goto } from "@roxi/routify";
    import { authUser } from "../../stores/auth";

    let checkForAdmin = true;
    $: if ($authUser && checkForAdmin) {
        if (!$authUser?.isAdmin) {
            $goto("/");
        } else {
            checkForAdmin = false;
        }
    }
</script>

<slot />
