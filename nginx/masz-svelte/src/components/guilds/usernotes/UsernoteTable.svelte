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
    import type { IUserNoteView } from "../../../models/api/IUserNoteView";
    import UserIcon from "../../discord/UserIcon.svelte";
    import { authUser, isModeratorInGuild } from "../../../stores/auth";
    import type { Writable } from "svelte/store";
    import { writable } from "svelte/store";
    import Autocomplete from "../../../core/Autocomplete.svelte";
    import { toastSuccess } from "../../../services/toast/store";

    let notes: Writable<IUserNoteView[]> = writable([]);
    let initialLoading: boolean = true;
    let loading: boolean = true;
    let fullSize: number = 0;
    let currentPage: number = 1;

    let userNoteModal: Writable<boolean> = writable(false);
    let userNoteModalSubmitting: Writable<boolean> = writable(false);
    let userNoteUserId: string = "";
    let userNoteText: string = "";
    let userNoteUserDisabled: Writable<boolean> = writable(false);

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
            notes.set([]);
        }
        lastUsedGuildId = $currentParams?.guildId;
        API.get(`/guilds/${$currentParams.guildId}/usernoteview?startPage=${page - 1}`, CacheMode.PREFER_CACHE, true)
            .then((response: { items: IUserNoteView[]; fullSize: number }) => {
                notes.set(response.items);
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
        API.clearCacheEntryLike("get", `/guilds/${$currentParams.guildId}/usernoteview`);
    }

    function openUpdateModal(note: IUserNoteView) {
        userNoteModalSubmitting.set(false);
        userNoteUserId = note.userNote.userId;
        userNoteText = note.userNote.description;
        userNoteUserDisabled.set(true);
        userNoteModal.set(true);
    }

    function deleteNote(note: IUserNoteView) {
        API.deleteData(`/guilds/${$currentParams.guildId}/usernote/${note.userNote.userId}`, {})
            .then(() => {
                notes.update((n) => {
                    return n.filter((x) => x.userNote.userId !== note.userNote.userId);
                });
                fullSize--;
                toastSuccess($_("guilds.usernote.usernotedeleted"));
                resetCache();
            })
            .catch(() => {
                toastError($_("guilds.usernote.usernotedeletefailed"));
            });
    }

    function updateUserNote() {
        const data = {
            userId: userNoteUserId,
            description: userNoteText,
        };
        userNoteModalSubmitting.set(true);
        API.put(`/guilds/${$currentParams.guildId}/usernote`, data)
            .then((res: IUserNoteView) => {
                notes.update((n) => {
                    const index = n.findIndex((x) => x.userNote.userId === res.userNote.userId);
                    if (index !== -1) {
                        toastSuccess($_("guilds.usernote.usernoteupdated"));
                        n[index] = res;
                    } else {
                        toastSuccess($_("guilds.usernote.usernotecreated"));
                        n.unshift(res);
                        fullSize++;
                    }
                    return n;
                });

                resetCache();
                onModalClose();
            })
            .catch(() => {
                userNoteModalSubmitting.set(false);
                toastError($_("guilds.usernote.usernotecreatefailed"));
            });
    }

    function onModalClose() {
        userNoteModal.set(false);
        userNoteModalSubmitting.set(false);
        userNoteUserId = "";
        userNoteText = "";
        userNoteUserDisabled.set(false);
    }
</script>

<Modal
    size="sm"
    open={$userNoteModal}
    selectorPrimaryFocus="#usernotememberselection"
    modalHeading={$_("guilds.usernote.createusernote")}
    primaryButtonText={$_("guilds.usernote.createusernote")}
    secondaryButtonText={$_("dialog.confirm.cancel")}
    on:close={onModalClose}
    on:click:button--secondary={onModalClose}
    on:submit={updateUserNote}>
    <Loading active={$userNoteModalSubmitting} />
    <div class="mb-4">
        <Autocomplete
            id="usernotememberselection"
            disabled={$userNoteUserDisabled}
            bind:selectedId={userNoteUserId}
            placeholder={$_("guilds.usernote.member")}
            items={members}
            valueMatchCustomValue={(value) => {
                return !isNaN(parseFloat(value)) && !isNaN(value - 0);
            }}
            warnText={members.length === 0 ? $_("guilds.usernote.nomembersfound") : ""} />
        <TextArea bind:value={userNoteText} placeholder={$_("guilds.usernote.createusernotedescription")} class="mt-2" rows={6} />
    </div>
</Modal>

<MediaQuery query="(min-width: 768px)" let:matches>
    <!-- Header -->
    <div class="flex flex-col mb-4">
        <h2 class="font-weight-bold mb-4">
            {$_("nav.guild.usernotes")}
        </h2>
        <div class="flex flex-row">
            <Button iconDescription={$_("guilds.usernote.createusernote")} icon={Add24} on:click={() => userNoteModal.set(true)} />
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
            <div class="text-lg font-bold">{$_("guilds.usernote.nomatches")}</div>
            <div class="text-md">{$_("guilds.usernote.nomatchesdescription")}</div>
        </div>
    {:else}
        <div class="grid gap-1 grid-cols-1">
            {#each $notes as note (note.userNote.id)}
                <Tile class="mb-2">
                    <div class="flex flex-row items-center">
                        <div class="grow flex-shrink">
                            <div class="flex flex-col grow flex-shrink">
                                <div class="flex flex-row items-center">
                                    <UserIcon user={note.user} />
                                    <div class="flex flex-col md:flex-row">
                                        <div class="ml-2 font-bold" style="word-wrap: anywhere">
                                            {#if note.user}
                                                {note.user.username}#{note.user.discriminator}
                                            {:else}
                                                {note.userNote.userId}
                                            {/if}
                                        </div>
                                        <div class="ml-2">
                                            {note.userNote.updatedAt.format($currentLanguage?.momentDateTimeFormat ?? "MMMM Do YYYY, h:mm:ss")}
                                        </div>
                                    </div>
                                </div>
                                <div class="mt-2">
                                    <span title={note.userNote.description} style="word-break: break-all; white-space: pre-wrap;">
                                        {note.userNote.description}
                                    </span>
                                </div>
                            </div>
                        </div>
                        {#if isModeratorInGuild($authUser, $currentParams.guildId)}
                            <div class="self-start">
                                <OverflowMenu flipped>
                                    <OverflowMenuItem
                                        on:click={() => {
                                            openUpdateModal(note);
                                        }}>
                                        {$_("core.edit")}
                                    </OverflowMenuItem>
                                    <OverflowMenuItem
                                        danger
                                        on:click={() => {
                                            deleteNote(note);
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
