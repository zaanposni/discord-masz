<script lang="ts">
    import { authUser } from "./../../stores/auth";
    import UserIcon from "./UserIcon.svelte";
</script>

<UserIcon user={$authUser?.discordUser} />
