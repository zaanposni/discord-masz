<script lang="ts">
    import ScheduledMessageFailureReasons from "./../../../services/enums/ScheduledMessageFailureReason";
    import { Add32, Edit20, Delete20 } from "carbon-icons-svelte";
    import {
        Button,
        ComboBox,
        DatePicker,
        DatePickerInput,
        Link,
        Loading,
        Modal,
        SkeletonPlaceholder,
        SkeletonText,
        TextArea,
        TextInput,
        Tile,
        TimePicker,
    } from "carbon-components-svelte";
    import API from "../../../services/api/api";
    import { CacheMode } from "../../../services/api/CacheMode";
    import UserIcon from "../../discord/UserIcon.svelte";
    import { currentParams } from "./../../../stores/currentParams";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import { Pagination } from "carbon-components-svelte";
    import { PaginationSkeleton } from "carbon-components-svelte";
    import { authUser } from "../../../stores/auth";
    import { _ } from "svelte-i18n";
    import Warning_02 from "carbon-pictograms-svelte/lib/Warning_02.svelte";
    import { toastError, toastSuccess } from "../../../services/toast/store";
    import type { IScheduledMessage } from "../../../models/api/IScheduledMessage";
    import type { IDiscordChannel } from "../../../models/discord/IDiscordChannel";
    import ScheduledMessageStatusTag from "../../api/ScheduledMessageStatusTag.svelte";
    import { currentLanguage, currentFlatpickrLocale } from "../../../stores/currentLanguage";
    import { writable } from "svelte/store";
    import type { Writable } from "svelte/store";
    import moment from "moment";
    import type { ILanguageSelect } from "../../../models/ILanguageSelect";
    import { confirmDialogReturnFunction, showConfirmDialog } from "../../../core/confirmDialog/store";
    import { ScheduledMessageStatus } from "../../../models/api/ScheduledMessageStatus";

    const utcOffset = new Date().getTimezoneOffset() * -1;

    const messages: Writable<IScheduledMessage[]> = writable([]);
    const channels: Writable<{ id: string; text: string }[]> = writable([]);
    let initialLoading: boolean = true;
    let loading: boolean = true;
    let fullSize: number = 0;
    let currentPage: number = 1;

    const showModal: Writable<boolean> = writable(false);
    const modalSubmitting: Writable<boolean> = writable(false);
    const modalModeEdit: Writable<boolean> = writable(false);
    const modalMessageId: Writable<number> = writable(0);
    const modalTitle: Writable<string> = writable("");
    const modalMessage: Writable<string> = writable("");
    const modalChannel: Writable<string> = writable("");
    const modalTime: Writable<moment.Moment> = writable(null);

    let inputPunishedUntilDate: any;
    let inputPunishedUntilTime: any;

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
            messages.set([]);
            page = 1;
        }
        lastUsedGuildId = $currentParams?.guildId;
        API.get(`/guilds/${$currentParams.guildId}/scheduledmessages?startPage=${page - 1}`, CacheMode.PREFER_CACHE, true)
            .then((response: { items: IScheduledMessage[]; fullSize: number }) => {
                messages.set(response.items);
                fullSize = response.fullSize;
                loading = false;
                initialLoading = false;
            })
            .catch(() => {
                loading = false;
                initialLoading = false;
                toastError($_("guilds.messages.failedtoload"));
            });
    }

    $: $currentParams?.guildId ? loadGuildData() : null;
    function loadGuildData() {
        API.get(`/discord/guilds/${$currentParams.guildId}/channels`, CacheMode.PREFER_CACHE, true).then((response: IDiscordChannel[]) => {
            channels.set(
                response
                    .filter((x) => x.type === 0)
                    .sort((a, b) => (a.position > b.position ? 1 : -1))
                    .map((x) => ({
                    id: x.id,
                    text: x.name,
                }))
            );
        });
    }

    function clearCache() {
        API.clearCacheEntryLike("get", `/guilds/${$currentParams.guildId}/scheduledmessages`);
    }

    function openModal() {
        modalSubmitting.set(false);
        showModal.set(true);
    }

    function editMessage(message: IScheduledMessage) {
        modalModeEdit.set(true);
        modalMessageId.set(message.id);
        modalTitle.set(message.name);
        modalMessage.set(message.content);
        modalChannel.set(message.channelId);

        inputPunishedUntilDate = message.scheduledFor.format($currentLanguage?.momentDateFormat ?? "YYYY-MM-DD");
        inputPunishedUntilTime = message.scheduledFor.format($currentLanguage?.momentTimeFormat ?? "HH:mm");

        openModal();
    }

    $: calculateScheduledFor(inputPunishedUntilDate, inputPunishedUntilTime, $currentLanguage);
    function calculateScheduledFor(date: string, time: string, language?: ILanguageSelect) {
        if (language) {
            modalTime.set(
                date
                    ? moment(`${date} ${time ? time : "00:00"}`, `${language.momentDateFormat} ${language.momentTimeFormat}`)
                          .utc(false)
                          .utcOffset(utcOffset)
                    : null
            );
        }
    }

    function onModalClose() {
        showModal.set(false);
        setTimeout(() => {
            modalSubmitting.set(false);
            modalModeEdit.set(false);
            modalTitle.set("");
            modalMessage.set("");
            modalChannel.set("");
            modalMessageId.set(0);

            inputPunishedUntilDate = null;
            inputPunishedUntilTime = null;
        }, 400);
    }

    function onModalSubmit() {
        modalSubmitting.set(true);
        if ($modalModeEdit) {
            API.put(`/guilds/${$currentParams.guildId}/scheduledmessages/${$modalMessageId}`, {
                name: $modalTitle,
                content: $modalMessage,
                channelId: $modalChannel,
                scheduledFor: $modalTime?.toISOString(),
            })
                .then((res) => {
                    toastSuccess($_("guilds.messages.savedmessage"));

                    messages.update((n) => {
                        const index = n.findIndex((x) => x.id === $modalMessageId);
                        if (index > -1) {
                            n[index] = res;
                        }
                        return n;
                    });

                    onModalClose();
                    clearCache();
                })
                .catch(() => {
                    toastError($_("guilds.messages.failedtosavemessage"));
                    modalSubmitting.set(false);
                });
        } else {
            API.post(
                `/guilds/${$currentParams.guildId}/scheduledmessages`,
                {
                    name: $modalTitle,
                    content: $modalMessage,
                    channelId: $modalChannel,
                    scheduledFor: $modalTime?.toISOString(),
                },
                CacheMode.API_ONLY,
                false
            )
                .then((res: IScheduledMessage) => {
                    toastSuccess($_("guilds.messages.savedmessage"));

                    messages.update((n) => {
                        n.unshift(res);
                        return n;
                    });
                    fullSize++;

                    onModalClose();
                    clearCache();
                })
                .catch(() => {
                    toastError($_("guilds.messages.failedtosavemessage"));
                    modalSubmitting.set(false);
                });
        }
    }

    function initDeleteModal(message: IScheduledMessage) {
        confirmDialogReturnFunction.set(deleteMessage.bind(this, message));
        showConfirmDialog.set(true);
    }

    function deleteMessage(message: IScheduledMessage) {
        API.deleteData(`/guilds/${$currentParams.guildId}/scheduledmessages/${message.id}`, {})
            .then(() => {
                toastSuccess($_("guilds.messages.messagedeleted"));

                messages.update((n) => {
                    return n.filter((x) => x.id !== message.id);
                });
                fullSize--;

                clearCache();
            })
            .catch(() => {
                toastError($_("guilds.messages.failedtodelete"));
            });
    }
</script>

<Modal
    size="sm"
    open={$showModal}
    selectorPrimaryFocus="#messagetitleselection"
    modalHeading={$_($modalModeEdit ? "guilds.messages.editmessage" : "guilds.messages.createnew")}
    primaryButtonText={$_("guilds.messages.savemessage")}
    secondaryButtonText={$_("dialog.confirm.cancel")}
    shouldSubmitOnEnter={false}
    primaryButtonDisabled={$modalSubmitting ||
        !$modalTitle ||
        !$modalMessage ||
        !$modalChannel ||
        !$modalTime ||
        ($modalTime && $modalTime.isBefore(moment().add(3, "minutes")))}
    on:close={onModalClose}
    on:click:button--secondary={onModalClose}
    on:submit={onModalSubmit}>
    <Loading active={$modalSubmitting} />
    <div class="mb-4">
        <TextInput id="messagetitleselection" bind:value={$modalTitle} placeholder={$_("guilds.messages.title")} />
    </div>
    <div class="mb-4">
        <ComboBox
            items={$channels}
            bind:selectedId={$modalChannel}
            placeholder={$_("guilds.messages.channel")}
            shouldFilterItem={(item, value) => {
                return `#${item.text.toLowerCase()}`.includes(value.toLowerCase());
            }} />
    </div>
    <div class="flex flex-row mb-4">
        <DatePicker
            class="!grow-0 !shrink mr-4"
            bind:value={inputPunishedUntilDate}
            datePickerType="single"
            locale={$currentFlatpickrLocale ?? "en"}
            dateFormat={$currentLanguage?.dateFormat ?? "m/d/Y"}
            flatpickrProps={{ static: true }}
            on:change>
            <DatePickerInput placeholder={$currentLanguage?.dateFormat ?? "m/d/Y"} />
        </DatePicker>
        <TimePicker
            class="!grow-0"
            bind:value={inputPunishedUntilTime}
            invalid={!!inputPunishedUntilTime && !/([01][012]|[1-9]):[0-5][0-9](\\s)?/.test(inputPunishedUntilTime)}
            invalidText={$_("guilds.casedialog.formatisrequired", { values: { format: $currentLanguage?.timeFormat ?? "hh:MM" } })}
            placeholder={$currentLanguage?.timeFormat ?? "hh:MM"} />
    </div>
    <div class="mb-4">
        <TextArea bind:value={$modalMessage} placeholder={$_("guilds.messages.content")} rows={6} />
    </div>
</Modal>

<MediaQuery query="(min-width: 768px)" let:matches>
    <!-- Header -->
    <div class="flex flex-col mb-4">
        <h2 class="font-weight-bold mb-4">
            {$_("nav.guild.messages")}
        </h2>
        <div class="flex flex-row">
            {#if $authUser?.adminGuilds?.find((x) => x.id === $currentParams.guildId) || $authUser?.modGuilds?.find((x) => x.id === $currentParams.guildId)}
                <Button class="!mr-2" icon={Add32} on:click={openModal}>{$_("guilds.messages.createnew")}</Button>
            {/if}
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
                                <div class="flex flex-row">
                                    <SkeletonPlaceholder class="mr-3" style="height: 4rem; width: 4rem" />
                                    <div class="flex grow flex-col flex-shrink">
                                        <div class="flex flex-row items-center">
                                            <SkeletonText width={"5%"} />
                                            <div class="mr-5" />
                                            <SkeletonText width={"20%"} />
                                        </div>
                                        <SkeletonText width={"50%"} />
                                    </div>
                                </div>
                                <SkeletonText paragraph lines={2} class="mt-2" />
                            </div>
                        </div>
                    </div>
                </Tile>
            {/each}
        </div>
    {:else if fullSize === 0}
        <div class="flex flex-col ml-2" class:items-center={!matches}>
            <Warning_02 />
            <div class="text-lg font-bold">{$_("guilds.messages.nomessages")}</div>
            <div class="text-md">{$_("guilds.messages.nomessagesdescription")}</div>
        </div>
    {:else}
        <div class="grid gap-1 grid-cols-1">
            {#each $messages as message (message.id)}
                <Tile class="mb-2">
                    <div class="flex flex-row items-center">
                        <div class="grow flex-shrink">
                            <div class="flex flex-col grow flex-shrink">
                                <div class="flex flex-row">
                                    <UserIcon
                                        size={matches ? 48 : 32}
                                        class="{matches ? 'self-center' : 'self-start'} mr-3"
                                        user={message.lastEdited} />
                                    <div class="flex flex-col flex-shrink">
                                        <div class="flex flex-row items-center flex-wrap mb-1">
                                            <ScheduledMessageStatusTag class="grow-0 shrink-0 !ml-0 mr-1" {message} />
                                            {#if message.status === ScheduledMessageStatus.Failed}
                                                <div class="mr-2">
                                                    {$_(ScheduledMessageFailureReasons.getById(message.failureReason))}
                                                </div>
                                            {/if}
                                            <div class="font-bold mr-2" style="word-wrap: anywhere">
                                                {message.lastEdited?.username}#{message.lastEdited?.discriminator}
                                            </div>
                                            <div style="word-wrap: anywhere">
                                                {message.scheduledFor
                                                    ? message.scheduledFor.format($currentLanguage?.momentDateTimeFormat ?? "DD/MM/YYYY HH:mm")
                                                    : ""}
                                            </div>
                                        </div>
                                        <div class="font-bold" style="word-wrap: anywhere" title={message.name}>
                                            #{message.channel?.name ?? "Unknown Channel"} - {message.name}
                                        </div>
                                    </div>
                                </div>
                                <div class="mt-2">
                                    <div title={message.content} style="word-wrap: anywhere; white-space: pre-wrap;">
                                        {message.content.length > (matches ? 1000 : 200)
                                            ? message.content.substring(0, matches ? 1000 : 200) + " [...]"
                                            : message.content}
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="flex flex-col self-start">
                            {#if message.status === ScheduledMessageStatus.Pending}
                                <Link
                                    class="mb-2 cursor-pointer"
                                    icon={Edit20}
                                    on:click={() => {
                                        editMessage(message);
                                    }} />
                            {/if}
                            <Link
                                class="cursor-pointer"
                                icon={Delete20}
                                on:click={() => {
                                    initDeleteModal(message);
                                }} />
                        </div>
                    </div>
                </Tile>
            {/each}
        </div>
    {/if}
</MediaQuery>
