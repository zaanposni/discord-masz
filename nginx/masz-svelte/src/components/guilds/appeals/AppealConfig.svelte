<script lang="ts">
    import { CacheMode } from "./../../../services/api/CacheMode";
    import { _ } from "svelte-i18n";
    import type { Writable } from "svelte/store";
    import { writable } from "svelte/store";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import type { IAppealStructure } from "../../../models/api/IAppealStructure";
    import { currentParams } from "../../../stores/currentParams";
    import API from "../../../services/api/api";
    import { Button, Modal, SkeletonText, TextArea, TextAreaSkeleton, Tile } from "carbon-components-svelte";
    import { flip } from "svelte/animate";
    import { dndzone } from "svelte-dnd-action";
    import { toastError, toastSuccess } from "../../../services/toast/store";
    import { Edit24, TrashCan24 } from "carbon-icons-svelte";

    const appealStructures: Writable<IAppealStructure[]> = writable([]);
    const loading: Writable<boolean> = writable(false);
    const submitting: Writable<boolean> = writable(false);
    const newQuestion: Writable<string> = writable("");
    const flipDurationMs = 300;

    const showEditModal: Writable<boolean> = writable(false);
    const editQuestion: Writable<IAppealStructure> = writable(null);

    $: $currentParams?.guildId ? loadData() : null;
    function loadData() {
        loading.set(true);

        API.get(`/guilds/${$currentParams.guildId}/appealstructures`, CacheMode.PREFER_CACHE, true)
            .then((response: IAppealStructure[]) => {
                appealStructures.set(response.sort((a, b) => a.sortOrder - b.sortOrder));
                loading.set(false);
            })
            .catch(() => {
                loading.set(false);
            });
    }

    function clearCache() {
        API.clearCacheEntry("get", `/guilds/${$currentParams.guildId}/appealstructures`);
    }

    function handleDndConsider(e) {
        appealStructures.set(e.detail.items);
    }

    function handleDndFinalize(e) {
        appealStructures.set(e.detail.items);
        submitting.set(true);

        const data = $appealStructures?.map((element, index) => {
            return {
                id: element.id,
                sortOrder: index,
            };
        });

        API.put(`/guilds/${$currentParams.guildId}/appealstructures/reorder`, data)
            .then(() => {
                submitting.set(false);
                clearCache();
            })
            .catch(() => {
                submitting.set(false);
                toastError($_("guilds.appealconfig.failedtoreorder"));
            });
    }

    function deleteQuestion(id: number) {
        const element = $appealStructures.find((element) => element.id == id);
        appealStructures.update((value) => value.filter((element) => element.id != id));

        API.deleteData(`/guilds/${$currentParams.guildId}/appealstructures/${id}`, {})
            .then(() => {
                toastSuccess($_("guilds.appealconfig.deleted"));
                clearCache();
            })
            .catch(() => {
                toastError($_("guilds.appealconfig.failedtodelete"));
                appealStructures.update((value) => [...value, element]);
            });
    }

    function addQuestion() {
        submitting.set(true);
        const data = {
            question: $newQuestion,
            sortOrder: Math.max(...(this.appealStructures ? this.appealStructures.map((x) => x.sortOrder) : [0]), 0) + 1,
        };

        API.post(`/guilds/${$currentParams.guildId}/appealstructures`, data, CacheMode.API_ONLY, false)
            .then((res) => {
                newQuestion.set("");
                clearCache();
                toastSuccess($_("guilds.appealconfig.saved"));
                appealStructures.update((x) => [...x, res]);
                submitting.set(false);
            })
            .catch(() => {
                toastError($_("guilds.appealconfig.failedtosave"));
                submitting.set(false);
            });
    }

    function onEdit(appeal: IAppealStructure) {
        editQuestion.set(appeal);
        showEditModal.set(true);
    }

    function onModalClose() {
        showEditModal.set(false);
        setTimeout(() => {
            editQuestion.set(null);
        }, 300);
    }

    function onModalConfirm() {
        submitting.set(true);
        API.put(`/guilds/${$currentParams.guildId}/appealstructures/${$editQuestion.id}`, $editQuestion)
            .then((res) => {
                clearCache();
                toastSuccess($_("guilds.appealconfig.saved"));

                appealStructures.update((x) => {
                    const index = x.findIndex((x) => x.id == $editQuestion.id);
                    if (index !== -1) {
                        x[index] = res;
                    }
                    return x;
                });

                submitting.set(false);
                onModalClose();
            })
            .catch(() => {
                toastError($_("guilds.appealconfig.failedtosave"));
                submitting.set(false);
            });
    }
</script>

<MediaQuery query="(min-width: 768px)" let:matches>
    <Modal
        shouldSubmitOnEnter={false}
        open={$showEditModal}
        on:click:button--primary={onModalConfirm}
        on:close={onModalClose}
        primaryButtonDisabled={!$editQuestion?.question}
        modalHeading={$_("guilds.appealconfig.editquestion")}
        primaryButtonText={$_("dialog.confirm.confirm")}>
        {#if $editQuestion}
            <TextArea
                label={$_("guilds.appealconfig.question")}
                placeholder={$_("guilds.appealconfig.question")}
                bind:value={$editQuestion.question}
                disabled={$submitting} />
        {/if}
    </Modal>

    <!-- Header -->
    <div class="flex flex-col {matches ? 'w-1/2' : ''}">
        <div class="text-3xl font-bold mb-4">
            {$_("guilds.appealconfig.title")}
        </div>
        <div class="text-md">{$_("guilds.appealconfig.explain")}</div>
        <div class="text-md">{$_("guilds.appealconfig.explain2")}</div>
        <div class="text-md">{$_("guilds.appealconfig.explain3")}</div>
        <div class="text-md mb-4">{$_("guilds.appealconfig.explain4")}</div>
    </div>
    <div class="w-full md:w-1/2">
        {#if $loading}
            <SkeletonText class="!mb-6" paragraph lines={2} />
            <SkeletonText class="!mb-6" paragraph lines={2} />
            <SkeletonText class="!mb-6" paragraph lines={2} />
            <hr class="my-2" />
            <TextAreaSkeleton />
            <div class="flex flex-row mt-2">
                <div class="grow" />
                <Button class="justify-self-end" skeleton />
            </div>
        {:else}
            <section
                class="grid gap-1 grid-cols-1"
                use:dndzone={{ items: $appealStructures, flipDurationMs, dropTargetStyle: {}, morphDisabled: true, dragDisabled: false }}
                on:consider={handleDndConsider}
                on:finalize={handleDndFinalize}>
                {#each $appealStructures as appealStructure (appealStructure.id)}
                    <div animate:flip={{ duration: flipDurationMs }}>
                        <Tile>
                            <div class="flex flex-row flex-nowrap">
                                <div class="grow text-2xl whitespace-pre-wrap" style="word-wrap: anywhere;">
                                    {appealStructure.question}
                                </div>
                                <div
                                    class="cursor-pointer shrink-0 ml-1"
                                    on:click={() => {
                                        onEdit(appealStructure);
                                    }}>
                                    <Edit24 />
                                </div>
                                <div
                                    class="cursor-pointer shrink-0 text-red-500 ml-1"
                                    on:click={() => {
                                        deleteQuestion(appealStructure.id);
                                    }}>
                                    <TrashCan24 />
                                </div>
                            </div>
                        </Tile>
                    </div>
                {/each}
                <hr class="my-2" />
                <TextArea
                    readonly={$submitting}
                    label={$_("guilds.appealconfig.question")}
                    placeholder={$_("guilds.appealconfig.question")}
                    bind:value={$newQuestion}
                    disabled={$submitting} />
                <div class="flex flex-row mt-2">
                    <div class="grow" />
                    <Button class="justify-self-end" disabled={!$newQuestion} on:click={addQuestion}>{$_("guilds.appealconfig.addquestion")}</Button>
                </div>
            </section>
        {/if}
    </div>
</MediaQuery>
