<script lang="ts">
    import { Button, SkeletonText, TextArea, Tile } from "carbon-components-svelte";
    import { ScalesTipped20 } from "carbon-icons-svelte";
    import { _ } from "svelte-i18n";
    import type { Writable } from "svelte/store";
    import { writable } from "svelte/store";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import { currentParams } from "../../../stores/currentParams";
    import { toastError, toastSuccess } from "../../../services/toast/store";
    import type { IAppealStructure } from "../../../models/api/IAppealStructure";
    import type { IAppealView } from "../../../models/api/IAppealView";
    import { goto } from "@roxi/routify";

    const loading: Writable<boolean> = writable(true);
    const submitting: Writable<boolean> = writable(false);
    const appealStructure: Writable<IAppealStructure[]> = writable([]);
    const answers: Writable<{ questionId: number; answer: string }[]> = writable([]);

    let inputPunishedUntilDate: any;

    $: $currentParams?.guildId ? loadData() : null;
    function loadData() {
        loading.set(true);

        API.get(`/guilds/${$currentParams.guildId}/appealstructures`, CacheMode.PREFER_CACHE, true)
            .then((res) => {
                appealStructure.set(res);
                answers.set(res.map((question) => ({ questionId: question.id, answer: "" })));
                loading.set(false);
            })
            .catch(() => {
                loading.set(false);
            });
    }

    function clearCache() {
        API.clearCacheEntryLike('post', `/guilds/${$currentParams.guildId}/appeal`);
    }

    function saveAppeal() {
        submitting.set(true);

        API.post(`/guilds/${$currentParams.guildId}/appeal`, { answers: $answers })
            .then((res: IAppealView) => {
                clearCache();
                toastSuccess($_("guilds.appealnew.saved"));
                submitting.set(false);
                $goto(`/guilds/${$currentParams.guildId}/appeals/${res.id}`);
            })
            .catch(() => {
                toastError($_("guilds.appealnew.failedtosave"));
                submitting.set(false);
            });
    }
</script>

<MediaQuery query="(min-width: 1024px)" let:matches>
    <div class="flex flex-col">
        <!-- Header -->
        <div class="flex flex-col">
            <!-- Icon -->
            <div class="flex flex-row grow items-center" style="color: var(--cds-text-02)">
                <ScalesTipped20 class="mr-2" />
                {#if $currentParams?.guild?.name && !$loading}
                    <div class="flex flex-row items-center">
                        {$currentParams.guild?.name}
                    </div>
                {:else}
                    <SkeletonText class="!mb-0" width="100px" />
                {/if}
            </div>

            <div class="my-2">
                <div>
                    {$_("guilds.appealnew.answer")}
                </div>
                <div>
                    {$_("guilds.appealnew.skip")}
                </div>
                <div>
                    {$_("guilds.appealnew.view")}
                </div>
            </div>

            <!-- Appeal -->
            <div class="w-full xl:w-1/2">
                {#if $loading}
                    <SkeletonText class="!mb-6" paragraph lines={2} />
                    <SkeletonText class="!mb-6" paragraph lines={2} />
                    <SkeletonText class="!mb-6" paragraph lines={2} />
                {:else}
                    {#each $appealStructure as question (question.id)}
                        <Tile class="mb-2">
                            <div class="flex flex-col">
                                <div class="text-2xl whitespace-pre-wrap" style="word-wrap: anywhere;">
                                    {question.question ?? "Unknown"}
                                </div>
                                <hr class="mt-4 mb-2" />

                                <TextArea
                                    class="w-full"
                                    disabled={$submitting}
                                    placeholder={$_("guilds.appealnew.answerplaceholder")}
                                    on:change={(e) => {
                                        answers.update((n) => {
                                            const index = n.findIndex((a) => a.questionId === question.id);

                                            if (index === -1) {
                                                n = [
                                                    ...n,
                                                    {
                                                        questionId: question.id,
                                                        answer: e.target.value,
                                                    },
                                                ];
                                            } else {
                                                n[index].answer = e.target.value;
                                            }

                                            return n;
                                        });
                                    }} />
                            </div>
                        </Tile>
                    {/each}
                {/if}
                <div class="flex flex-row mt-2">
                    <div class="grow" />
                    <Button class="justify-self-end" disabled={$submitting} on:click={saveAppeal}>{$_("guilds.appealnew.save")}</Button>
                </div>
            </div>
        </div>
    </div>
</MediaQuery>
