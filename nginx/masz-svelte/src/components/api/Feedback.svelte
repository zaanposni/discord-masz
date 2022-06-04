<script lang="ts">
    import { FaceDissatisfied32, FaceDissatisfiedFilled32, FaceNeutral32, FaceNeutralFilled32, FaceSatisfied32, FaceSatisfiedFilled32, FaceWink20, FaceWink32 } from "carbon-icons-svelte";
    import MediaQuery from "../../core/MediaQuery.svelte";
    import { FEEDBACK_COOKIE_NAME } from "../../config";
    import Cookies from "../../services/cookie";
    import type { IFeedback } from "../../models/api/IFeedback";
    import type { Writable } from "svelte/store";
    import { writable } from "svelte/store";
    import { Loading, Modal, TextArea, Tile } from "carbon-components-svelte";
    import { _ } from "svelte-i18n";
    import { authUser } from "../../stores/auth";
    import API from "../../services/api/api";
    import { toastSuccess } from "../../services/toast/store";

    let providedFeedback = Cookies.getCookie(FEEDBACK_COOKIE_NAME);

    const feedback: Writable<IFeedback> = writable({
        rating: 1,
        comment: "",
        remote: window.location.host,
        user: "",
    });
    const submitting: Writable<boolean> = writable(false);
    const modalOpen: Writable<boolean> = writable(false);

    function onModalClear() {
        modalOpen.set(false);
        setTimeout(() => {
            submitting.set(false);
            feedback.set({
                rating: 1,
                comment: "",
                remote: window.location.host,
                user: $authUser.discordUser.username + "#" + $authUser.discordUser.discriminator,
            });
        }, 200);
    }

    function submitFeedback() {
        submitting.set(true);
        API.post("/meta/feedback", {
            ...$feedback,
            user: $authUser.discordUser.username + "#" + $authUser.discordUser.discriminator,
        }).finally(() => {
            toastSuccess($_("feedback.thanks"));
            onModalClear();
            providedFeedback = "true";
            Cookies.setCookie(FEEDBACK_COOKIE_NAME, "true", 90);
        });
    }
</script>

<style>
    .float {
        position: fixed;
        background-color: var(--cds-interactive-01);
        color: #fff;
        border-radius: 50px;
        display: flex;
        flex-direction: row;
        justify-content: center;
        align-items: center;
    }

    .float.big {
        width: 64px;
        height: 64px;
        bottom: 40px;
        right: 40px;
    }

    .float.small {
        width: 40px;
        height: 40px;
        bottom: 20px;
        right: 20px;
    }

    .float:hover {
        background-color: var(--cds-interactive-04);
    }

    .experience-option {
        background-color: var(--cds-ui-01, #ffffff);
    }

    .experience-option:hover {
        background-color: var(--cds-ui-03, #ffffff);
        cursor: pointer;
    }
</style>

{#if !providedFeedback}
    <MediaQuery query="(min-width: 768px)" let:matches>
        <div
            class="float cursor-pointer"
            class:small={!matches}
            class:big={matches}
            on:click={() => {
                modalOpen.set(true);
            }}>
            {#if matches}
                <FaceWink32 />
            {:else}
                <FaceWink20 />
            {/if}
        </div>
    </MediaQuery>
    {#if $feedback}
        <Modal
            size="sm"
            shouldSubmitOnEnter={false}
            open={$modalOpen}
            selectorPrimaryFocus="#feedbackselection"
            modalHeading={$_("feedback.title")}
            primaryButtonText={$_("feedback.submit")}
            primaryButtonDisabled={$submitting || $feedback?.comment?.length > 4096}
            secondaryButtonText={$_("feedback.cancel")}
            on:close={onModalClear}
            on:click:button--secondary={onModalClear}
            on:submit={submitFeedback}>
            <Loading active={$submitting} />
            <div class="flex flex-col">
                <div class="mb-2">{$_("feedback.experience")}</div>
                <div class="flex flex-row flex-nowrap w-full mb-4">
                    <div
                        class="experience-option p-8"
                        on:click={() => {
                            $feedback.rating = 0;
                        }}>
                        {#if $feedback?.rating === 0}
                            <FaceDissatisfiedFilled32 />
                        {:else}
                            <FaceDissatisfied32 />
                        {/if}
                    </div>
                    <div
                        class="experience-option p-8"
                        on:click={() => {
                            $feedback.rating = 1;
                        }}>
                        {#if $feedback?.rating === 1}
                            <FaceNeutralFilled32 id="feedbackselection" />
                        {:else}
                            <FaceNeutral32 />
                        {/if}
                    </div>
                    <div
                        class="experience-option p-8"
                        on:click={() => {
                            $feedback.rating = 2;
                        }}>
                        {#if $feedback?.rating === 2}
                            <FaceSatisfiedFilled32 />
                        {:else}
                            <FaceSatisfied32 />
                        {/if}
                    </div>
                </div>
                <div class="mb-2">{$_("feedback.comment")}</div>
                <TextArea
                    bind:value={$feedback.comment}
                    disabled={$submitting}
                    invalid={$feedback?.comment?.length > 4096}
                    invalidText={$_("feedback.commenttoolong")}
                    placeholder={$_("feedback.comment")}
                    rows={5}
                    class="w-full" />
            </div>
        </Modal>
    {/if}
{/if}
