<script lang="ts">
    import { slide } from "svelte/transition";
    import { API_URL } from "../../../config";
    import {
        Button,
        Checkbox,
        CopyButton,
        InlineNotification,
        Loading,
        Modal,
        OverflowMenu,
        OverflowMenuItem,
        ProgressBar,
        SkeletonPlaceholder,
        SkeletonText,
        Tag,
        TextArea,
        TextInput,
        Tile,
    } from "carbon-components-svelte";
    import {
        Add24,
        ChevronDown24,
        ChevronUp24,
        Edit24,
        List24,
        Locked24,
        Power24,
        Send24,
        Share24,
        TrashCan24,
        Upload24,
        WatsonHealthAiStatusComplete24,
    } from "carbon-icons-svelte";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import type { ICaseView } from "../../../models/api/ICaseView";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import { authUser, isModeratorInGuild } from "../../../stores/auth";
    import { currentParams } from "../../../stores/currentParams";
    import PunishmentTag from "../../api/PunishmentTag.svelte";
    import type { IComment } from "../../../models/api/IComment";
    import UserIcon from "../../discord/UserIcon.svelte";
    import { currentLanguage } from "../../../stores/currentLanguage";
    import { toastError, toastSuccess } from "../../../services/toast/store";
    import type { Writable } from "svelte/store";
    import { writable } from "svelte/store";
    import { goto } from "@roxi/routify";
    import { confirmDialogReturnFunction, showConfirmDialog } from "../../../core/confirmDialog/store";
    import moment from "moment";
    import { PunishmentType } from "../../../models/api/PunishmentType";
    import { _ } from "svelte-i18n";
    import type { ICase } from "../../../models/api/ICase";

    const preloadFileExtensions = ["img", "png", "jpg", "jpeg", "gif", "webp"];

    let openFurtherDetails: boolean = false;

    let fileUploadRef: HTMLInputElement | null = null;

    let caseLoading: boolean = true;
    let modCase: Writable<ICaseView> = writable(null);
    let filesLoading: boolean = true;
    let files: Writable<{ fullName: string; fileName: string }[]> = writable([]);
    let renderedDescription: string = "";

    let newComment: string = "";
    let editCommentModal: Writable<boolean> = writable(false);
    let editCommentId: Writable<number> = writable(null);
    let editCommentValue: Writable<string> = writable("");
    let editCommentSubmitting: Writable<boolean> = writable(false);

    let deleteCaseModalOpen: Writable<boolean> = writable(false);
    let deleteCaseSubmitting: Writable<boolean> = writable(false);
    let deleteCaseModalPublicNotification: Writable<boolean> = writable(false);
    let deleteCaseModalForceDelete: Writable<boolean> = writable(false);

    $: $currentParams?.guildId && $currentParams?.caseId ? loadData() : null;
    function loadData() {
        caseLoading = true;
        filesLoading = true;
        API.get(`/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/view`, CacheMode.PREFER_CACHE, true)
            .then((response: ICaseView) => {
                response.linkedCases = [{ ...response.modCase }, { ...response.modCase }, { ...response.modCase }];
                response.linkedCases[0].title += response.modCase.title;
                response.linkedCases[1].title = "lol";
                modCase.set(response);
                renderDescription(response?.modCase?.description ?? "");
                caseLoading = false;
            })
            .catch(() => {
                $goto("/guilds/" + $currentParams.guildId + "/cases");
                toastError("Case not found.");
            });
        API.get(`/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/files`, CacheMode.PREFER_CACHE, true)
            .then((response: { names: string[] }) => {
                files.set(
                    response.names.map((x) => {
                        const fileName = x.split("_").slice(2).join("_");
                        const fullName = x;
                        return { fullName, fileName };
                    })
                );
                filesLoading = false;
            })
            .catch(() => {
                filesLoading = false;
            });
    }

    function clearCaseCache() {
        API.clearCacheEntry("get", `/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/view`);
        API.clearCacheEntryLike("post", `/guilds/${$currentParams.guildId}/modcasetable`);
    }

    function clearFileCache() {
        API.clearCacheEntry("get", `/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/files`);
    }

    function renderDescription(description: string) {
        let value = description.replace("<", "&lt;").replace(">", "&gt;").replace(/\n/g, "<br>");
        value = value.replace(/#([\d]+)/g, `<a href=/guilds/${$currentParams.guildId}/cases/$1>#$1</a>`);
        renderedDescription = value;
    }

    function deleteFile(file: string) {
        API.deleteData(`/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/files/${file}`, {})
            .then(() => {
                toastSuccess("File deleted");
                clearFileCache();
                files.update((x) => x.filter((y) => y.fullName !== file));
            })
            .catch(() => {
                toastError("Failed to delete file");
            });
    }

    function unlinkCase(caseId: number) {
        console.log("unlinkCase", caseId);
    }

    function linkCase() {
        console.log("linkCase");
    }

    function deleteComment(id: number) {
        API.deleteData(`/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/comments/${id}`, {})
            .then(() => {
                toastSuccess("Comment deleted");
                modCase.update((x) => {
                    x.comments = x.comments.filter((y) => y.comment.id !== id);
                    return x;
                });
                clearCaseCache();
            })
            .catch(() => {
                toastError("Failed to delete comment");
            });
    }

    function sendComment() {
        if ((newComment?.trim()?.length ?? 0) === 0) return;
        const data = {
            message: newComment,
        };

        API.post(`/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/comments`, data, CacheMode.API_ONLY, false)
            .then((res: IComment) => {
                newComment = "";
                modCase.update((x) => {
                    x.comments.push({
                        commentor: $authUser.discordUser,
                        comment: res,
                    });
                    return x;
                });
                toastSuccess("Comment added!");
                clearCaseCache();
            })
            .catch(() => {
                toastError("Failed to add comment");
            });
    }

    function shareCase() {
        navigator.clipboard
            .writeText(window.location.toString())
            .then(() => {
                toastSuccess("Link copied to clipboard");
            })
            .catch((e) => console.error(e));
    }

    function uploadFile() {
        fileUploadRef.onchange = () => {
            for (let index = 0; index < fileUploadRef.files.length; index++) {
                const file = fileUploadRef.files[index];
                const formData = new FormData();
                formData.append("file", file);
                API.post(`/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/files`, formData, CacheMode.API_ONLY, false)
                    .then((res: { path: string }) => {
                        const fileName = res.path.split("_").slice(2).join("_");
                        const fullName = res.path;
                        files.update((n) => {
                            n.push({
                                fullName,
                                fileName,
                            });
                            return n;
                        });
                        toastSuccess("File uploaded!");
                        clearFileCache();
                    })
                    .catch(() => {
                        toastError("Failed to upload file");
                    });
            }
        };
        fileUploadRef.click();
    }

    function handleActivation() {
        const handleActivationInternal = (confirmed) => {
            if (confirmed) {
                if ($modCase.modCase.punishmentActive) {
                    API.deleteData(`/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/active`, {})
                        .then(() => {
                            modCase.update((x) => {
                                x.modCase.punishmentActive = false;
                                return x;
                            });
                            toastSuccess("Punishment deactivated");
                            clearCaseCache();
                        })
                        .catch(() => {
                            toastError("Failed to deactivate punishment");
                        });
                } else {
                    API.post(`/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/active`, {}, CacheMode.API_ONLY, false)
                        .then(() => {
                            modCase.update((x) => {
                                x.modCase.punishmentActive = true;
                                return x;
                            });
                            toastSuccess("Punishment activated");
                            clearCaseCache();
                        })
                        .catch(() => {
                            toastError("Failed to activate punishment");
                        });
                }
            }
        };

        confirmDialogReturnFunction.set(handleActivationInternal);
        showConfirmDialog.set(true);
    }

    function lockComments() {
        API.post(`/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/lock`, {}, CacheMode.API_ONLY, false)
            .then(() => {
                modCase.update((x) => {
                    x.modCase.allowComments = false;
                    return x;
                });
                toastSuccess("Comments locked");
                clearCaseCache();
            })
            .catch(() => {
                toastError("Failed to lock comments");
            });
    }

    function unlockComments() {
        API.deleteData(`/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/lock`, {})
            .then(() => {
                modCase.update((x) => {
                    x.modCase.allowComments = true;
                    return x;
                });
                toastSuccess("Comments unlocked");
                clearCaseCache();
            })
            .catch(() => {
                toastError("Failed to unlock comments");
            });
    }

    function onDeleteCaseModalClose() {
        deleteCaseModalOpen.set(false);
        deleteCaseModalForceDelete.set(false);
        deleteCaseModalPublicNotification.set(false);
        deleteCaseSubmitting.set(false);
    }

    function deleteCaseModal() {
        deleteCaseModalForceDelete.set(false);
        deleteCaseModalPublicNotification.set(false);
        deleteCaseSubmitting.set(false);
        deleteCaseModalOpen.set(true);
    }

    function deleteCase() {
        deleteCaseSubmitting.set(true);
        API.deleteData(
            `/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}?sendnotification=${$deleteCaseModalPublicNotification}&forceDelete=${$deleteCaseModalForceDelete}`,
            {}
        )
            .then((res: ICase) => {
                clearCaseCache();
                if ($deleteCaseModalForceDelete) {
                    $goto("/guilds/" + $currentParams.guildId + "/cases");
                    toastSuccess("Case deleted");
                } else {
                    modCase.update((x) => {
                        x.modCase.markedToDeleteAt = res.markedToDeleteAt;
                        x.modCase.deletedByUserId = res.deletedByUserId;
                        x.deletedBy = $authUser.discordUser;
                        return x;
                    });
                    toastSuccess("Case marked to be deleted");
                }
                onDeleteCaseModalClose();
            })
            .catch(() => {
                deleteCaseSubmitting.set(false);
                toastError("Failed to delete case");
            });
    }

    function restoreCase() {
        API.deleteData(`/guilds/${$currentParams.guildId}/bin/${$currentParams.caseId}/restore`, {})
            .then(() => {
                modCase.update((x) => {
                    x.modCase.markedToDeleteAt = null;
                    x.modCase.deletedByUserId = null;
                    x.modCase.deletedByUserId = null;
                    return x;
                });
                toastSuccess("Case restored");
                clearCaseCache();
            })
            .catch(() => {
                toastError("Failed to restore case");
            });
    }

    function onEditCommentModalClose() {
        editCommentModal.set(false);
        editCommentSubmitting.set(false);
        editCommentId.set(null);
        editCommentValue.set(null);
    }

    function editCommentModalOpen(comment: IComment) {
        editCommentSubmitting.set(false);
        editCommentId.set(comment.id);
        editCommentValue.set(comment.message);
        editCommentModal.set(true);
    }

    function editComment() {
        editCommentSubmitting.set(true);
        const data = {
            message: $editCommentValue,
        };
        API.put(`/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/comments/${$editCommentId}`, data)
            .then(() => {
                toastSuccess("Comment edited");
                modCase.update((n) => {
                    const index = n.comments.findIndex((x) => x.comment.id === $editCommentId);
                    if (index !== -1) {
                        n.comments[index].comment.message = $editCommentValue;
                    }
                    return n;
                });
                onEditCommentModalClose();
                clearCaseCache();
            })
            .catch(() => {
                editCommentSubmitting.set(false);
                toastError("Failed to edit comment");
            });
    }
</script>

<style>
    :global(#casedescription a) {
        color: var(--cds-link-01) !important;
    }
    :global(#linkedcases .bx--tile) {
        display: flex;
    }
    :global(#caseview-labelist .bx--tag:first-child) {
        margin-left: 0;
    }
    :global(#caseview-punishment .bx--tag) {
        margin-left: 0;
    }
</style>

<!-- Edit comment modal -->
<Modal
    size="sm"
    open={$editCommentModal}
    selectorPrimaryFocus="#commentvalue"
    modalHeading={$_("guilds.casedialog.createtemplate")}
    primaryButtonText={$_("guilds.casedialog.createtemplate")}
    secondaryButtonText={$_("dialog.confirm.cancel")}
    primaryButtonDisabled={$editCommentValue !== undefined && $editCommentValue !== null && ($editCommentValue?.length ?? 0) > 300}
    on:close={onEditCommentModalClose}
    on:click:button--secondary={onEditCommentModalClose}
    on:submit={editComment}>
    <Loading active={$editCommentSubmitting} />
    <div class="mb-4">
        <TextInput
            id="commentvalue"
            bind:value={$editCommentValue}
            labelText={$_("guilds.casedialog.title")}
            placeholder={$_("guilds.casedialog.titleplaceholder")}
            required
            invalid={$editCommentValue !== undefined && $editCommentValue !== null && ($editCommentValue?.length ?? 0) > 300}
            invalidText={$_("guilds.casedialog.titleinvalid")} />
    </div>
</Modal>

<!-- Delete case modal -->
<Modal
    size="sm"
    danger
    open={$deleteCaseModalOpen}
    selectorPrimaryFocus="#publicnotification"
    modalHeading={$_("guilds.casedialog.createtemplate")}
    primaryButtonText={$_("guilds.casedialog.createtemplate")}
    secondaryButtonText={$_("dialog.confirm.cancel")}
    primaryButtonDisabled={$deleteCaseSubmitting}
    on:close={onDeleteCaseModalClose}
    on:click:button--secondary={onDeleteCaseModalClose}
    on:submit={deleteCase}>
    <Loading active={$deleteCaseSubmitting} />
    <div class="mb-4">
        <Checkbox id="publicnotification" labelText="Send public notification" bind:checked={$deleteCaseModalPublicNotification} />
        <Checkbox labelText="Force delete" bind:checked={$deleteCaseModalForceDelete} />
    </div>
</Modal>

<MediaQuery query="(min-width: 1024px)" let:matches>
    <div class="flex flex-col">
        <!-- Header -->
        <div class="flex flex-col">
            <!-- Icon -->
            <div class="flex flex-row grow items-center" style="color: var(--cds-text-02)">
                <List24 class="mr-2" />
                {#if $currentParams?.guild?.name && $currentParams?.caseId && !caseLoading}
                    <div class="flex flex-row items-center">
                        {$currentParams.guild?.name}-{$currentParams.caseId}
                    </div>
                {:else}
                    <SkeletonText class="!mb-0" width="100px" />
                {/if}
            </div>
            <!-- Title -->
            <div class="flex flex-col mb-4">
                <div class="mb-4">
                    {#if caseLoading}
                        <SkeletonText heading paragraph lines={2} />
                    {:else}
                        <h2 class="font-black" style="word-wrap: anywhere">
                            {$modCase?.modCase?.title}
                        </h2>
                    {/if}
                </div>
                <div class="flex flex-row flex-wrap items-center">
                    {#if caseLoading}
                        <div class="mr-2 mb-2">
                            <Button size="small" skeleton />
                        </div>
                        <div class="mr-2 mb-2">
                            <Button size="small" skeleton />
                        </div>
                        <div class="mr-2 mb-2">
                            <Button size="small" skeleton />
                        </div>
                    {:else}
                        <div class:mb-2={matches && isModeratorInGuild($authUser, $currentParams.guildId)} class:mr-2={matches}>
                            <Button size="small" kind="secondary" icon={Share24} on:click={shareCase}>Share case</Button>
                        </div>
                        <input type="file" bind:this={fileUploadRef} name="fileUpload" multiple={true} accept="image/*" style="display:none;" />
                        {#if matches}
                            {#if isModeratorInGuild($authUser, $currentParams.guildId)}
                                <div class="mr-2 mb-2">
                                    <Button
                                        size="small"
                                        kind="secondary"
                                        icon={Upload24}
                                        disabled={$modCase.modCase.markedToDeleteAt !== null}
                                        on:click={uploadFile}>Upload file</Button>
                                </div>
                                {#if ($modCase.modCase.punishedUntil === null || $modCase?.modCase?.punishedUntil?.isAfter(moment())) && ($modCase.modCase.punishmentType == PunishmentType.Ban || $modCase.modCase.punishmentType == PunishmentType.Mute)}
                                    <div class="mr-2 mb-2">
                                        <Button
                                            size="small"
                                            kind="secondary"
                                            icon={Power24}
                                            disabled={$modCase.modCase.markedToDeleteAt !== null}
                                            on:click={handleActivation}>{$modCase.modCase.punishmentActive ? "Deactivate" : "Activate"}</Button>
                                    </div>
                                {/if}
                                <div class="mr-2 mb-2">
                                    {#if $modCase.modCase.allowComments}
                                        <Button
                                            size="small"
                                            kind="secondary"
                                            icon={Locked24}
                                            disabled={$modCase.modCase.markedToDeleteAt !== null}
                                            on:click={lockComments}>Lock comments</Button>
                                    {:else}
                                        <Button
                                            size="small"
                                            kind="secondary"
                                            icon={Locked24}
                                            disabled={$modCase.modCase.markedToDeleteAt !== null}
                                            on:click={unlockComments}>Unlock comments</Button>
                                    {/if}
                                </div>
                                <div class="mr-2 mb-2">
                                    <Button
                                        size="small"
                                        kind="secondary"
                                        icon={Edit24}
                                        disabled={$modCase.modCase.markedToDeleteAt !== null}
                                        on:click={() => {
                                            $goto(`/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/edit`);
                                        }}>Edit</Button>
                                </div>
                                {#if $modCase.modCase.markedToDeleteAt === null}
                                    <div class="mr-2 mb-2">
                                        <Button size="small" kind="danger" icon={TrashCan24} on:click={deleteCaseModal}>Delete</Button>
                                    </div>
                                {:else}
                                    <div class="mr-2 mb-2">
                                        <Button size="small" icon={WatsonHealthAiStatusComplete24} on:click={restoreCase}>Restore</Button>
                                    </div>
                                {/if}
                            {/if}
                        {:else}
                            <div class="grow" />
                            <OverflowMenu flipped>
                                <OverflowMenuItem text="Upload file" disabled={$modCase.modCase.markedToDeleteAt !== null} on:click={uploadFile} />
                                {#if ($modCase.modCase.punishedUntil === null || $modCase?.modCase?.punishedUntil?.isAfter(moment())) && ($modCase.modCase.punishmentType == PunishmentType.Ban || $modCase.modCase.punishmentType == PunishmentType.Mute)}
                                    <OverflowMenuItem
                                        text="Deactivate"
                                        disabled={$modCase.modCase.markedToDeleteAt !== null}
                                        on:click={handleActivation} />
                                {/if}
                                {#if $modCase.modCase.allowComments}
                                    <OverflowMenuItem
                                        text="Lock comments"
                                        disabled={$modCase.modCase.markedToDeleteAt !== null}
                                        on:click={lockComments} />
                                {:else}
                                    <OverflowMenuItem
                                        text="Unlock comments"
                                        disabled={$modCase.modCase.markedToDeleteAt !== null}
                                        on:click={unlockComments} />
                                {/if}
                                <OverflowMenuItem
                                    text="Edit"
                                    disabled={$modCase.modCase.markedToDeleteAt !== null}
                                    on:click={() => {
                                        $goto(`/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/edit`);
                                    }} />
                                {#if $modCase.modCase.markedToDeleteAt === null}
                                    <OverflowMenuItem text="Delete" danger on:click={deleteCaseModal} />
                                {:else}
                                    <OverflowMenuItem text="Restore" on:click={restoreCase} />
                                {/if}
                            </OverflowMenu>
                        {/if}
                    {/if}
                </div>
            </div>
        </div>
        <div class="flex flex-col lg:flex-row grow">
            <!-- Case content -->
            <div class="flex flex-col grow shrink-0 {matches ? 'pr-4 w-2/3' : ''}">
                {#if $modCase?.modCase?.markedToDeleteAt}
                    <InlineNotification
                        kind="info"
                        title="This case has been marked to be deleted at {$modCase?.modCase?.markedToDeleteAt?.format(
                            $currentLanguage?.momentDateTimeFormat ?? 'DD/MM/YYYY HH:mm'
                        )}"
                        subtitle={$modCase?.deletedBy
                            ? `Deleted by ${$modCase?.deletedBy?.username}#${$modCase?.deletedBy?.discriminator}`
                            : "Deleted by a moderator"} />
                {/if}
                {#if caseLoading}
                    <SkeletonText paragraph />
                {:else}
                    <div class="mb-4" id="casedescription">{@html renderedDescription}</div>
                {/if}
                <!-- Files -->
                {#if filesLoading}
                    <SkeletonText />
                    <div class="flex flex-row">
                        <div class="m-2">
                            <SkeletonPlaceholder />
                        </div>
                        <div class="m-2">
                            <SkeletonPlaceholder />
                        </div>
                        <div class="m-2">
                            <SkeletonPlaceholder />
                        </div>
                    </div>
                {:else if ($files?.length ?? 0) !== 0}
                    <div class="mb-4">
                        <div class="mb-2 font-bold">Attachments</div>
                        <div class="flex flex-row overflow-x-auto overflow-y-clip pb-2">
                            {#each $files as file}
                                <div class="grow shrink-0 h-80 max-h-80">
                                    {#if preloadFileExtensions.includes(file.fileName.split(".").pop())}
                                        <Tile class="flex flex-col overflow-hidden h-full p-4 mr-2">
                                            {#if isModeratorInGuild($authUser, $currentParams.guildId)}
                                                <div class="flex flex-row justify-end w-full mb-2">
                                                    <div
                                                        class="cursor-pointer"
                                                        on:click={() => {
                                                            deleteFile(file.fullName);
                                                        }}>
                                                        <TrashCan24 />
                                                    </div>
                                                </div>
                                            {/if}
                                            <img
                                                src={`${API_URL}/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/files/${file.fullName}`}
                                                alt="uploaded file {file.fileName}"
                                                style="max-height: {isModeratorInGuild($authUser, $currentParams.guildId) ? '90%' : '100%'};" />
                                        </Tile>
                                    {:else}
                                        <Tile class="flex flex-col overflow-hidden h-full p-4 mr-2">
                                            {#if isModeratorInGuild($authUser, $currentParams.guildId)}
                                                <div class="flex flex-row justify-end w-full mb-2">
                                                    <div
                                                        class="cursor-pointer"
                                                        on:click={() => {
                                                            deleteFile(file.fullName);
                                                        }}>
                                                        <TrashCan24 />
                                                    </div>
                                                </div>
                                            {/if}
                                            <a
                                                href={`${API_URL}/guilds/${$currentParams.guildId}/cases/${$currentParams.caseId}/files/${file.fullName}`}
                                                target="_blank"
                                                class="w-full">
                                                <div style="word-wrap: anywhere">
                                                    {file.fileName}
                                                </div>
                                            </a>
                                        </Tile>
                                    {/if}
                                </div>
                            {/each}
                        </div>
                    </div>
                {/if}

                <!-- Linked cases -->

                {#if caseLoading}
                    <div class="flex flex-row">
                        <SkeletonText width={"30%"} />
                        <div class="grow" />
                        <SkeletonText width={"5%"} />
                    </div>
                    <div class="flex flex-col">
                        <div class="m-2">
                            <SkeletonText />
                        </div>
                        <div class="m-2">
                            <SkeletonText />
                        </div>
                        <div class="m-2">
                            <SkeletonText />
                        </div>
                    </div>
                {:else}
                    <div class="mb-4">
                        <div class="flex flex-row mb-2">
                            <div class="font-bold self-end">Linked cases</div>
                            <div class="grow" />
                            {#if isModeratorInGuild($authUser, $currentParams.guildId)}
                                <Add24 class="cursor-pointer" on:click={linkCase} />
                            {/if}
                        </div>
                        <div class="flex flex-col" id="linkedcases">
                            {#each $modCase?.linkedCases ?? [] as linked}
                                <Tile class="mb-2">
                                    <div class="flex flex-row grow-0 w-full max-w-full items-center">
                                        <List24 class="shrink-0 mr-2" />
                                        <div class="shrink-0 mr-2" style="color: var(--cds-text-02)">
                                            #{linked.caseId}
                                        </div>
                                        <div class="grow truncate">
                                            {linked.title}
                                        </div>
                                        <div class="grow" />
                                        <div class="shrink-0">
                                            <PunishmentTag modCase={linked} />
                                        </div>
                                        <div>
                                            <OverflowMenu flipped>
                                                <OverflowMenuItem
                                                    text="Show case"
                                                    href={`/guilds/${$currentParams.guildId}/cases/${linked.caseId}`} />
                                                <OverflowMenuItem
                                                    text="Unlink case"
                                                    on:click={() => {
                                                        unlinkCase(linked.caseId);
                                                    }} />
                                            </OverflowMenu>
                                        </div>
                                    </div>
                                </Tile>
                            {/each}
                        </div>
                    </div>
                {/if}

                <!-- Comments -->

                {#if caseLoading}
                    <div class="flex flex-row">
                        <SkeletonText width={"30%"} />
                    </div>
                    <div class="flex flex-col">
                        <div class="m-2">
                            <SkeletonText />
                        </div>
                        <div class="m-2">
                            <SkeletonText />
                        </div>
                        <div class="m-2">
                            <SkeletonText />
                        </div>
                    </div>
                {:else}
                    <div class="mb-4">
                        <div class="flex flex-row mb-2">
                            <div class="font-bold">Comments</div>
                        </div>
                        <div class="flex flex-col" id="linkedcases">
                            {#each $modCase?.comments ?? [] as comment (comment.comment.id)}
                                <Tile class="mb-2">
                                    <div class="flex flex-row grow-0 w-full max-w-full">
                                        <UserIcon user={comment.commentor} class="self-start mr-2" />
                                        <div class="flex flex-col grow" style="min-width: 0;">
                                            <div class="flex flex-col md:flex-row grow mb-2">
                                                {#if comment.commentor}
                                                    <div class="font-bold">{comment.commentor.username}#{comment.commentor.discriminator}</div>
                                                {:else}
                                                    <div class="font-bold">The moderators</div>
                                                {/if}
                                                <div class="md:ml-2" style="color: var(--cds-text-02)">
                                                    {comment.comment.createdAt.format(
                                                        $currentLanguage?.momentDateTimeFormat ?? "MMMM Do YYYY, h:mm:ss"
                                                    )}
                                                </div>
                                            </div>
                                            <div class="grow" style="white-space: pre-wrap; word-wrap: anywhere;">
                                                {comment.comment.message}
                                            </div>
                                        </div>
                                        {#if $authUser.discordUser.id === comment.comment.userId || isModeratorInGuild($authUser, $currentParams.guildId)}
                                            <div class="shrink-0">
                                                <OverflowMenu flipped>
                                                    {#if $authUser.discordUser.id === comment.comment.userId}
                                                        <OverflowMenuItem
                                                            text="Edit"
                                                            disabled={!$modCase.modCase.allowComments}
                                                            on:click={() => {
                                                                editCommentModalOpen(comment.comment);
                                                            }} />
                                                    {/if}
                                                    {#if $authUser.discordUser.id === comment.comment.userId || isModeratorInGuild($authUser, $currentParams.guildId)}
                                                        <OverflowMenuItem
                                                            text="Delete"
                                                            disabled={!$modCase.modCase.allowComments}
                                                            on:click={() => {
                                                                deleteComment(comment.comment.id);
                                                            }} />
                                                    {/if}
                                                </OverflowMenu>
                                            </div>
                                        {/if}
                                    </div>
                                </Tile>
                            {/each}

                            {#if $modCase?.comments?.length}
                                <hr class="mb-2" style="border-color: var(--cds-ui-04)" />
                            {/if}

                            {#if !caseLoading}
                                <div class="flex flex-row items-center">
                                    <UserIcon user={$authUser.discordUser} class="mr-2" />
                                    <div class="mr-2 grow">
                                        <TextArea
                                            rows={2}
                                            bind:value={newComment}
                                            placeholder="Add a comment... (max 300 characters)"
                                            invalid={(newComment?.length ?? 0) > 300}
                                            invalidText="Comment must be less than 300 characters"
                                            disabled={!$modCase.modCase.allowComments ||
                                                $modCase.modCase.markedToDeleteAt !== null ||
                                                ($modCase?.comments?.at(-1)?.comment?.userId === $authUser?.discordUser?.id &&
                                                    !isModeratorInGuild($authUser, $currentParams.guildId))} />
                                    </div>
                                    <Send24
                                        class={!$modCase.modCase.allowComments ||
                                        $modCase.modCase.markedToDeleteAt !== null ||
                                        newComment?.trim()?.length === 0 ||
                                        (newComment?.trim()?.length ?? 0) > 300 ||
                                        ($modCase?.comments?.at(-1)?.comment?.userId === $authUser?.discordUser?.id &&
                                            !isModeratorInGuild($authUser, $currentParams.guildId))
                                            ? ""
                                            : "cursor-pointer"}
                                        on:click={sendComment} />
                                </div>
                                {#if !$modCase.modCase.allowComments}
                                    <InlineNotification kind="info" title="Locked:" subtitle="You cannot comment since this case is locked." />
                                {/if}
                            {/if}
                        </div>
                    </div>
                {/if}
            </div>
            <!-- Meta data -->
            <div class="flex flex-col grow shrink-0 {matches ? 'pl-4 w-1/3' : ''}" class:-order-1={!matches}>
                {#if caseLoading}
                    <div class="flex flex-row lg:flex-col">
                        <SkeletonText width={"10%"} />
                        <SkeletonText width={"30%"} />
                    </div>
                    <div class="flex flex-row lg:flex-col">
                        <SkeletonText width={"10%"} />
                        <SkeletonText width={"30%"} />
                    </div>
                    <div class="flex flex-row lg:flex-col">
                        <SkeletonText width={"10%"} />
                        <SkeletonText width={"30%"} />
                    </div>
                {:else}
                    {#if !matches}
                        <div class="flex flex-row justify-end mb-6">
                            <Button
                                size="small"
                                iconDescription=""
                                icon={openFurtherDetails ? ChevronUp24 : ChevronDown24}
                                on:click={() => {
                                    openFurtherDetails = !openFurtherDetails;
                                }} />
                        </div>
                    {/if}
                    {#if matches || openFurtherDetails}
                        <div class="flex flex-col grow shrink-0" transition:slide|local>
                            <div class="flex flex-col mb-6">
                                <div class="font-bold mb-2">Violator</div>
                                <div class="flex flex-row items-center">
                                    <UserIcon class="mr-2" user={$modCase.suspect} />
                                    <div class="flex flex-row flex-wrap items-center">
                                        <div class="mr-2">
                                            {$modCase.suspect?.username ?? $modCase.modCase.username}#{$modCase.suspect?.discriminator ??
                                                $modCase.modCase.discriminator}
                                        </div>
                                        <div class="mr-2" style="color: var(--cds-text-02)">
                                            ({$modCase.modCase.userId})
                                        </div>
                                    </div>
                                    <div class="grow" />
                                    <div>
                                        <CopyButton text={$modCase.modCase.userId} feedback="Copied to clipboard" />
                                    </div>
                                </div>
                            </div>
                            <div class="flex flex-col mb-6">
                                <div class="font-bold mb-2">Punishment {$modCase.modCase.punishmentActive ? "" : "(inactive)"}</div>
                                <div id="caseview-punishment">
                                    <PunishmentTag modCase={$modCase.modCase} />
                                </div>
                            </div>
                            {#if $modCase.modCase.labels.length !== 0}
                                <div class="flex flex-col mb-6">
                                    <div class="font-bold mb-2">Labels</div>
                                    <div class="flex flex-row flex-wrap" id="caseview-labelist">
                                        {#each $modCase.modCase.labels as label}
                                            <Tag type="outline">{label}</Tag>
                                        {/each}
                                    </div>
                                </div>
                            {/if}
                            {#if $modCase.modCase.punishedUntil}
                                <div class="flex flex-col mb-6">
                                    <div class="font-bold mb-2">Punished until</div>
                                    <div class="flex flex-col">
                                        {#if $modCase.modCase.punishedUntil}
                                            <ProgressBar class="mb-2" value={$modCase.punishmentProgress ?? 100} />
                                            <div>
                                                {$modCase.modCase.punishedUntil?.format(
                                                    $currentLanguage?.momentDateTimeFormat ?? "MMMM Do YYYY, h:mm:ss"
                                                ) ?? "Permanent"}
                                            </div>
                                        {/if}
                                    </div>
                                </div>
                            {/if}
                            {#if $modCase?.userNote?.userNote}
                                <div class="flex flex-col mb-6">
                                    <div class="font-bold mb-2">Usernote</div>
                                    <div style="white-space: pre-wrap; word-wrap: anywhere;">
                                        {$modCase.userNote.userNote.description}
                                    </div>
                                </div>
                            {/if}
                            <div class="flex flex-col mb-6">
                                <div class="font-bold mb-2">Moderator</div>
                                {#if $modCase.modCase.modId}
                                    <div class="flex flex-row items-center">
                                        <UserIcon class="mr-2" user={$modCase.moderator} />
                                        <div class="flex flex-row flex-wrap">
                                            {#if $modCase.moderator}
                                                <div class="mr-2">
                                                    {$modCase.moderator?.username}#{$modCase.moderator?.discriminator}
                                                </div>
                                            {/if}
                                            <div class="mr-2" style="color: var(--cds-text-02)">
                                                ({$modCase.modCase.modId})
                                            </div>
                                        </div>
                                        <div class="grow" />
                                        <div>
                                            <CopyButton text={$modCase.modCase.modId} feedback="Copied to clipboard" />
                                        </div>
                                    </div>
                                {:else}
                                    Unknown or hidden.
                                {/if}
                            </div>
                            {#if !$modCase.modCase.createdAt.isSame($modCase.modCase.lastEditedAt)}
                                <div class="flex flex-col mb-6">
                                    <div class="font-bold mb-2">Last edit by moderator</div>
                                    {#if $modCase.modCase.lastEditedByModId}
                                        <div class="flex flex-row items-center">
                                            <UserIcon class="mr-2" user={$modCase.lastModerator} />
                                            <div class="flex flex-row flex-wrap">
                                                {#if $modCase.lastModerator}
                                                    <div class="mr-2">
                                                        {$modCase.lastModerator?.username}#{$modCase.lastModerator?.discriminator}
                                                    </div>
                                                {/if}
                                                <div class="mr-2" style="color: var(--cds-text-02)">
                                                    ({$modCase.modCase.lastEditedByModId})
                                                </div>
                                            </div>
                                            <div class="grow" />
                                            <div>
                                                <CopyButton text={$modCase.modCase.lastEditedByModId} feedback="Copied to clipboard" />
                                            </div>
                                        </div>
                                    {:else}
                                        Unknown or hidden.
                                    {/if}
                                </div>
                            {/if}
                            <hr class="mb-6" style="border-color: var(--cds-ui-04)" />
                            <div class="flex flex-row mb-2" style="color: var(--cds-text-02)">
                                <div class="mr-2">Created</div>
                                <div>
                                    {$modCase.modCase.createdAt?.format($currentLanguage?.momentDateTimeFormat ?? "MMMM Do YYYY, h:mm:ss") ??
                                        "Unknown"}
                                </div>
                            </div>
                            {#if !$modCase.modCase.createdAt.isSame($modCase.modCase.lastEditedAt)}
                                <div class="flex flex-row" style="color: var(--cds-text-02)">
                                    <div class="mr-2">Updated</div>
                                    <div>
                                        {$modCase.modCase.lastEditedAt?.format($currentLanguage?.momentDateTimeFormat ?? "MMMM Do YYYY, h:mm:ss") ??
                                            "Unknown"}
                                    </div>
                                </div>
                            {/if}
                        </div>
                    {/if}
                {/if}
            </div>
        </div>
    </div>
</MediaQuery>
