<script lang="ts">
    import type { IDiscordUser } from "./../../../models/discord/IDiscordUser";
    import { Add24, Delete16, Edit16, Filter24 } from "carbon-icons-svelte";
    import { Button, Loading, Modal, OverflowMenu, OverflowMenuItem, SkeletonText, TextArea, Tile } from "carbon-components-svelte";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import { currentParams } from "./../../../stores/currentParams";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import { Pagination } from "carbon-components-svelte";
    import { PaginationSkeleton } from "carbon-components-svelte";
    import { _ } from "svelte-i18n";
    import Warning_02 from "carbon-pictograms-svelte/lib/Warning_02.svelte";
    import { toastError } from "../../../services/toast/store";
    import { currentLanguage } from "../../../stores/currentLanguage";
    import type { IUserMappingView } from "../../../models/api/IUserMappingView";
    import UserIcon from "../../discord/UserIcon.svelte";
    import { authUser, isModeratorInGuild } from "../../../stores/auth";
    import type { Writable } from "svelte/store";
    import { writable } from "svelte/store";
    import Autocomplete from "../../../core/Autocomplete.svelte";
    import { toastSuccess } from "../../../services/toast/store";

    let mappings: Writable<IUserMappingView[]> = writable([]);
    let initialLoading: boolean = true;
    let loading: boolean = true;
    let fullSize: number = 0;
    let currentPage: number = 1;

    let userMapModal: Writable<boolean> = writable(false);
    let userMapModalSubmitting: Writable<boolean> = writable(false);
    let userMapUserIdA: string = "";
    let userMapUserIdB: string = "";
    let userMapText: string = "";
    let userMapUserDisabled: Writable<boolean> = writable(false);

    let members: { id: string; text: string }[] = [];

    $: forwardText = $_("core.pagination.forwardtext");
    $: backwardText = $_("core.pagination.backwardtext");
    $: itemRangeText = (min, max, total) => $_("core.pagination.itemrangetext", { values: { min, max, total } });
    $: pageRangeText = (current, total) => $_(`core.pagination.pagerangetext${total === 1 ? "" : "plural"}`, { values: { total } });

    let lastUsedGuildId: string = "";
    $: $currentParams?.guildId && currentPage ? loadData(currentPage) : null;
    function loadData(page: number = 1) {
        loading = true;
        if (lastUsedGuildId !== "" && lastUsedGuildId !== $currentParams?.guildId) {
            // reset on guild change
            currentPage = 1;
            page = 1;
            mappings.set([]);
        }
        lastUsedGuildId = $currentParams?.guildId;
        API.get(`/guilds/${$currentParams.guildId}/usermapview?startPage=${page - 1}`, CacheMode.PREFER_CACHE, true)
            .then((response: { items: IUserMappingView[]; fullSize: number }) => {
                mappings.set(response.items);
                fullSize = response.fullSize;
                loading = false;
                initialLoading = false;
            })
            .catch(() => {
                loading = false;
                initialLoading = false;
                toastError($_("guilds.automodtable.failedtoload"));
            });
    }

    $: $currentParams?.guildId ? loadGuildData() : null;
    function loadGuildData() {
        API.get(`/discord/guilds/${$currentParams.guildId}/members`, CacheMode.PREFER_CACHE, true).then((response: IDiscordUser[]) => {
            members = response.map((x) => ({
                id: x.id,
                text: `${x.username}#${x.discriminator}`,
            }));
        });
    }

    function resetCache() {
        API.clearCacheEntryLike("get", `/guilds/${$currentParams.guildId}/usermapview`);
    }

    function openUpdateModal(usermap: IUserMappingView) {
        userMapModalSubmitting.set(false);
        userMapUserIdA = usermap.userMapping.userA;
        userMapUserIdB = usermap.userMapping.userB;
        userMapText = usermap.userMapping.reason;
        userMapUserDisabled.set(true);
        userMapModal.set(true);
    }

    function deleteMap(usermap: IUserMappingView) {
        API.deleteData(`/guilds/${$currentParams.guildId}/usermap/${usermap.userMapping.id}`, {})
            .then(() => {
                mappings.update((n) => {
                    return n.filter((x) => x.userMapping.id !== usermap.userMapping.id);
                });
                fullSize--;
                toastSuccess($_("guilds.usermap.usermapdeleted"));
                resetCache();
            })
            .catch(() => {
                toastError($_("guilds.usermap.usermapdeletefailed"));
            });
    }

    function updateusermap() {
        const data = {
            userA: userMapUserIdA,
            userB: userMapUserIdB,
            reason: userMapText,
        };
        userMapModalSubmitting.set(true);
        API.put(`/guilds/${$currentParams.guildId}/usermap`, data)
            .then((res: IUserMappingView) => {
                mappings.update((n) => {
                    const index = n.findIndex((x) => x.userMapping.id === res.userMapping.id);
                    if (index !== -1) {
                        toastSuccess($_("guilds.usermap.usermapupdated"));
                        n[index] = res;
                    } else {
                        toastSuccess($_("guilds.usermap.usermapcreated"));
                        n.unshift(res);
                        fullSize++;
                    }
                    return n;
                });

                resetCache();
                onModalClose();
            })
            .catch(() => {
                userMapModalSubmitting.set(false);
                toastError($_("guilds.usermap.usermapcreatefailed"));
            });
    }

    function onModalClose() {
        userMapModal.set(false);
        userMapModalSubmitting.set(false);
        userMapUserIdA = "";
        userMapUserIdB = "";
        userMapText = "";
        userMapUserDisabled.set(false);
    }
</script>

<Modal
    size="sm"
    open={$userMapModal}
    selectorPrimaryFocus="#usermapmemberselection"
    modalHeading={$_("guilds.usermap.createusermap")}
    primaryButtonText={$_("guilds.usermap.createusermap")}
    secondaryButtonText={$_("dialog.confirm.cancel")}
    on:close={onModalClose}
    on:click:button--secondary={onModalClose}
    on:submit={updateusermap}>
    <Loading active={$userMapModalSubmitting} />
    <div class="mb-4">
        <div>
            <Autocomplete
                id="usermapmemberselection"
                disabled={$userMapUserDisabled}
                bind:selectedId={userMapUserIdA}
                placeholder={$_("guilds.usermap.member")}
                items={members}
                valueMatchCustomValue={(value) => {
                    return !isNaN(parseFloat(value)) && !isNaN(value - 0);
                }}
                warnText={members.length === 0 ? $_("guilds.usermap.nomembersfound") : ""} />
        </div>
        <div class="mt-2">
            <Autocomplete
                id="usermapmemberselection"
                disabled={$userMapUserDisabled}
                bind:selectedId={userMapUserIdB}
                placeholder={$_("guilds.usermap.member")}
                items={members}
                valueMatchCustomValue={(value) => {
                    return !isNaN(parseFloat(value)) && !isNaN(value - 0);
                }}
                warnText={members.length === 0 ? $_("guilds.usermap.nomembersfound") : ""} />
        </div>
        <TextArea bind:value={userMapText} placeholder={$_("guilds.usermap.createusermapdescription")} class="mt-2" rows={6} />
    </div>
</Modal>

<MediaQuery query="(min-width: 768px)" let:matches>
    <!-- Header -->
    <div class="flex flex-col mb-4">
        <h2 class="font-weight-bold mb-4">
            {$_("nav.guild.usermaps")}
        </h2>
        <div class="flex flex-row">
            <Button iconDescription={$_("guilds.usermap.createusermap")} icon={Add24} on:click={() => userMapModal.set(true)} />
        </div>
    </div>
    {#if initialLoading}
        <PaginationSkeleton class="mb-4" />
    {:else}
        <Pagination
            class="mb-4"
            disabled={loading}
            bind:page={currentPage}
            pageSize={20}
            totalItems={fullSize}
            pageSizeInputDisabled
            {forwardText}
            {backwardText}
            {itemRangeText}
            {pageRangeText} />
    {/if}
    <!-- Table -->
    {#if loading}
        <div class="grid gap-1 grid-cols-1">
            {#each { length: 3 } as _, i}
                <Tile class="mb-2">
                    <div class="flex flex-row items-center">
                        <div class="grow flex-shrink">
                            <div class="flex flex-col grow flex-shrink">
                                <div class="flex flex-row items-center">
                                    <SkeletonText width={"5%"} />
                                    <div class="mr-5" />
                                    <SkeletonText width={"20%"} />
                                </div>
                                <SkeletonText paragraph lines={4} class="mt-2" />
                            </div>
                        </div>
                    </div>
                </Tile>
            {/each}
        </div>
    {:else if fullSize === 0}
        <div class="flex flex-col ml-2" class:items-center={!matches}>
            <Warning_02 />
            <div class="text-lg font-bold">{$_("guilds.usermap.nomatches")}</div>
            <div class="text-md">{$_("guilds.usermap.nomatchesdescription")}</div>
        </div>
    {:else}
        <div class="grid gap-1 grid-cols-1">
            {#each $mappings as usermap (usermap.userMapping.id)}
                <Tile class="mb-2">
                    <div class="flex flex-row items-center">
                        <div class="grow flex-shrink">
                            <div class="flex flex-col grow flex-shrink">
                                <div class="flex flex-row items-center">
                                    <div class="mr-2">
                                        <UserIcon user={usermap.userA} />
                                    </div>
                                    <div class="mr-2">
                                        <UserIcon user={usermap.userB} />
                                    </div>
                                    <div class="flex flex-col md:flex-row">
                                        <div class="font-bold mr-2">
                                            {#if usermap.userA}
                                                {usermap.userA.username}#{usermap.userA.discriminator}
                                            {:else}
                                                {usermap.userMapping.userA}
                                            {/if}
                                            -
                                        </div>
                                        <div class="font-bold mr-2">
                                            {#if usermap.userB}
                                                {usermap.userB.username}#{usermap.userB.discriminator}
                                            {:else}
                                                {usermap.userMapping.userB}
                                            {/if}
                                        </div>
                                        <div>
                                            {usermap.userMapping.createdAt.format($currentLanguage?.momentDateTimeFormat ?? "MMMM Do YYYY, h:mm:ss")}
                                        </div>
                                    </div>
                                </div>
                                <div class="mt-2">
                                    <span title={usermap.userMapping.reason} style="word-break: break-all; white-space: pre-wrap;">
                                        {usermap.userMapping.reason}
                                    </span>
                                </div>
                            </div>
                        </div>
                        {#if isModeratorInGuild($authUser, $currentParams.guildId)}
                            <div class="self-start">
                                <OverflowMenu flipped>
                                    <OverflowMenuItem
                                        on:click={() => {
                                            openUpdateModal(usermap);
                                        }}>
                                        {$_("core.edit")}
                                    </OverflowMenuItem>
                                    <OverflowMenuItem
                                        danger
                                        on:click={() => {
                                            deleteMap(usermap);
                                        }}>
                                        {$_("core.delete")}
                                    </OverflowMenuItem>
                                </OverflowMenu>
                            </div>
                        {/if}
                    </div>
                </Tile>
            {/each}
        </div>
    {/if}
</MediaQuery>
