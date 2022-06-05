<script lang="ts">
    import { slide } from "svelte/transition";
    import { Button, Checkbox, DatePicker, DatePickerInput, Select, SelectItem, SkeletonText, TextArea, Tile } from "carbon-components-svelte";
    import { ScalesTipped20 } from "carbon-icons-svelte";
    import { _ } from "svelte-i18n";
    import type { Writable } from "svelte/store";
    import { writable } from "svelte/store";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import { AppealStatus } from "../../../models/api/AppealStatus";
    import AppealStatusEnum from "../../../services/enums/AppealStatus";
    import type { IAppealView } from "../../../models/api/IAppealView";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import { authUser, isModeratorInGuild } from "../../../stores/auth";
    import { currentLanguage } from "../../../stores/currentLanguage";
    import { currentParams } from "../../../stores/currentParams";
    import AppealStatusTag from "../../api/AppealStatusTag.svelte";
    import { currentFlatpickrLocale } from "../../../stores/currentLanguage";
    import moment from "moment";
    import type { ILanguageSelect } from "../../../models/ILanguageSelect";
    import { toastError, toastSuccess } from "../../../services/toast/store";

    const utcOffset = new Date().getTimezoneOffset() * -1;
    const loading: Writable<boolean> = writable(true);
    const submitting: Writable<boolean> = writable(false);
    const appeal: Writable<IAppealView> = writable(null);

    let inputPunishedUntilDate: any;

    $: $currentParams?.guildId ? loadData() : null;
    function loadData() {
        loading.set(true);

        API.get(`/guilds/${$currentParams.guildId}/appeal/${$currentParams.appealId}`, CacheMode.PREFER_CACHE, true)
            .then((response: IAppealView) => {
                appeal.set({
                    ...response,
                    status: response.status.toString(),
                } as any);
                inputPunishedUntilDate = response.userCanCreateNewAppeals
                    ? moment(response.userCanCreateNewAppeals).format($currentLanguage?.momentDateFormat ?? "YYYY-MM-DD")
                    : "";
                loading.set(false);
            })
            .catch(() => {
                loading.set(false);
            });
    }

    function clearCache() {
        API.clearCacheEntryLike('post', `/guilds/${$currentParams.guildId}/appeal`);
        API.clearCacheEntry("get", `/guilds/${$currentParams.guildId}/appeal/${$currentParams.appealId}`);
    }

    $: calculatePunishedUntil(inputPunishedUntilDate, $currentLanguage);
    function calculatePunishedUntil(date: string, language?: ILanguageSelect) {
        if (language && $appeal) {
            appeal.update((n) => {
                n.userCanCreateNewAppeals = date
                    ? moment(`${date} 00:00"`, `${language.momentDateFormat} ${language.momentTimeFormat}`).utc(false).utcOffset(utcOffset)
                    : null;
                return n;
            });
        }
    }

    function saveAppeal() {
        submitting.set(true);

        API.put(`/guilds/${$currentParams.guildId}/appeal/${$currentParams.appealId}`, $appeal)
            .then((res) => {
                toastSuccess($_("guilds.appealview.saved"));
                appeal.set({
                    ...res,
                    status: res.status.toString(),
                });
                submitting.set(false);
                clearCache();
            })
            .catch(() => {
                toastError($_("guilds.appealview.failedtosave"));
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
                {#if $currentParams?.guild?.name && $currentParams?.appealId && !$loading}
                    <div class="flex flex-row items-center">
                        {$currentParams.guild?.name}-{$currentParams.appealId}
                    </div>
                {:else}
                    <SkeletonText class="!mb-0" width="100px" />
                {/if}
            </div>

            <!-- Title -->
            <div class="flex flex-col mb-4">
                {#if $loading}
                    <SkeletonText heading />
                {:else}
                    <div class="flex flex-col md:flex-row md:items-center">
                        <h2 class="font-black" style="word-wrap: anywhere">
                            {$appeal?.user?.username ?? $appeal?.username}#{$appeal?.user?.discriminator ?? $appeal?.discriminator}
                        </h2>
                        <div class="md:ml-2">
                            <AppealStatusTag appeal={$appeal} />
                        </div>
                    </div>
                {/if}
            </div>

            <!-- Metadata -->
            <div class="flex flex-col mb-4">
                {#if $loading}
                    <SkeletonText class="!mb-0" width="300px" />
                {:else}
                    <div class="grid grid-cols-2 gap-2 lg:w-1/4">
                        <div>{$_("guilds.appealview.createdat")}:</div>
                        <div>{$appeal.createdAt.format($currentLanguage?.momentDateTimeFormat ?? "DD/MM/YYYY HH:mm")}</div>
                        {#if $appeal.updatedAt}
                            <div class="mt-1">{$_("guilds.appealview.updatedat")}:</div>
                            <div>{$appeal.updatedAt.format($currentLanguage?.momentDateTimeFormat ?? "DD/MM/YYYY HH:mm")}</div>
                        {/if}
                    </div>
                {/if}
            </div>

            <!-- Appeal -->
            <div class="w-full xl:w-1/2">
                    {#if $loading}
                        <SkeletonText class="!mb-6" paragraph lines={2} />
                        <SkeletonText class="!mb-6" paragraph lines={2} />
                        <SkeletonText class="!mb-6" paragraph lines={2} />
                    {:else}
                        {#each $appeal.answers as answer (answer.id)}
                            <Tile class="mb-2">
                                <div class="flex flex-col">
                                    <div class="text-2xl whitespace-pre-wrap" style="word-wrap: anywhere;">
                                        {$appeal.structures.find((s) => s.id === answer.questionId)?.question ?? "Unknown"}
                                    </div>
                                    <hr class="mt-4 mb-2" />
                                    <div class="whitespace-pre-wrap" style="word-wrap: anywhere;">
                                        {answer.answer ? answer.answer : $_("guilds.appealview.noanswer")}
                                    </div>
                                </div>
                            </Tile>
                        {/each}
                    {/if}

                <hr class="my-4" />

                <!-- Result -->
                <Tile>
                    <div class="flex flex-col">
                        {#if $loading}
                            <SkeletonText />
                            <div class="flex flex-row w-full mb-6">
                                <div class="grow" />
                                <SkeletonText />
                                <div class="grow" />
                            </div>
                            <SkeletonText heading />
                            <SkeletonText class="!mb-6" />
                            <SkeletonText heading />
                            <SkeletonText class="!mb-6" />
                        {:else}
                            {#if $appeal.status != AppealStatus.Pending}
                                <div class="text-2xl font-semibold">
                                    {$_("guilds.appealview.decisionat")}: {$appeal.updatedAt.format(
                                        $currentLanguage?.momentDateTimeFormat ?? "DD/MM/YYYY HH:mm"
                                    )}
                                </div>
                            {/if}
                            <div class="flex flex-row w-full">
                                <div class="grow" />
                                <AppealStatusTag class="!p-4 !text-4xl" appeal={$appeal} />
                                <div class="grow" />
                            </div>
                            {#if $appeal.moderatorComment}
                                <div class="text-2xl font-semibold mt-4">
                                    {$_("guilds.appealview.moderatorcomment")}:
                                </div>
                                <div class="whitespace-pre-wrap" style="word-wrap: anywhere;">
                                    {$appeal.moderatorComment}
                                </div>
                            {/if}
                            {#if $appeal.status != AppealStatus.Pending}
                                <div class="text-2xl font-semibold mt-4">
                                    {$_("guilds.appealview.newappeal")}
                                </div>
                                {#if $appeal.userCanCreateNewAppeals === null}
                                    <div>
                                        {$_("guilds.appealview.newappealnever")}
                                    </div>
                                {:else}
                                    <div>
                                        {$_("guilds.appealview.newappealat")}: {$appeal.userCanCreateNewAppeals.format(
                                            $currentLanguage?.momentDateTimeFormat ?? "DD/MM/YYYY HH:mm"
                                        )}
                                    </div>
                                {/if}
                            {/if}
                        {/if}
                    </div>
                </Tile>

                <!-- if user is admin or mod -->
                {#if !$loading && isModeratorInGuild($authUser, $currentParams.guildId)}
                    <hr class="my-4" />

                    <!-- Actions -->
                    <div class="flex flex-col">
                        <div class="text-2xl font-semibold">
                            {$_("guilds.appealview.edit")}
                        </div>
                        <div>
                            <div class="xl:w-1/2 mt-4">
                                <Select bind:selected={$appeal.status} hideLabel readonly={$submitting}>
                                    {#each AppealStatusEnum.getAll() as type (type)}
                                        <SelectItem value={type.id.toString()} text={$_(type.translationKey)} />
                                    {/each}
                                </Select>
                            </div>
                            <div class="mt-4">
                                <TextArea
                                    bind:value={$appeal.moderatorComment}
                                    labelText={$_("guilds.appealview.moderatorcomment")}
                                    placeholder={$_("guilds.appealview.moderatorcomment")}
                                    readonly={$submitting} />
                            </div>
                            {#if $appeal.status == AppealStatus.Denied}
                                <div transition:slide|local>
                                    <div class="xl:w-1/2 mt-4">
                                        <Checkbox
                                            checked={$appeal.userCanCreateNewAppeals === null}
                                            labelText={$_("guilds.appealview.newappealnever")}
                                            readonly={$submitting}
                                            on:check={(e) => {
                                                appeal.update((n) => {
                                                    n.userCanCreateNewAppeals = e.detail ? null : moment();
                                                    return n;
                                                });
                                                if (!e.detail) {
                                                    inputPunishedUntilDate = moment($appeal.userCanCreateNewAppeals).format(
                                                        $currentLanguage?.momentDateFormat ?? "YYYY-MM-DD"
                                                    );
                                                }
                                            }} />
                                    </div>
                                    {#if $appeal.userCanCreateNewAppeals !== null}
                                        <div class="xl:w-1/2 mt-4" class:flex-wrap={!matches} transition:slide|local>
                                            <DatePicker
                                                bind:value={inputPunishedUntilDate}
                                                datePickerType="single"
                                                locale={$currentFlatpickrLocale ?? "en"}
                                                dateFormat={$currentLanguage?.dateFormat ?? "m/d/Y"}
                                                on:change>
                                                <DatePickerInput
                                                    labelText={$_("guilds.casedialog.punisheduntil")}
                                                    placeholder={$currentLanguage?.dateFormat ?? "m/d/Y"} />
                                            </DatePicker>
                                        </div>
                                    {/if}
                                </div>
                            {/if}
                            <div class="flex flex-row mt-2">
                                <div class="grow" />
                                <Button class="justify-self-end" disabled={$submitting || !$appeal.moderatorComment} on:click={saveAppeal}
                                    >{$_("guilds.appealview.edit")}</Button>
                            </div>
                        </div>
                    </div>
                {/if}
            </div>
        </div>
    </div>
</MediaQuery>
