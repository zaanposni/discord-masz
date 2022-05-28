<script lang="ts">
    import { Button, Checkbox, Select, SelectItem, SelectSkeleton, SkeletonText, TextArea, TextInput, TextInputSkeleton } from "carbon-components-svelte";
    import { _ } from "svelte-i18n";
    import { derived, Writable } from "svelte/store";
    import { writable } from "svelte/store";
    import MediaQuery from "../../core/MediaQuery.svelte";
    import type { IMASZSettings } from "../../models/api/IMASZSettings";
    import API from "../../services/api/api";
    import { CacheMode } from "../../services/api/CacheMode";
    import { toastError, toastSuccess } from "../../services/toast/store";
    import Languages from "../../services/enums/Language";

    const settings: Writable<IMASZSettings> = writable(null);
    const submitting: Writable<boolean> = writable(false);

    const webhookRegex = new RegExp("^https://discord(app)?.com/api/webhooks/.*$");

    const embedTitleInvalid = derived(settings, (data) => {
        if (!data) return true;
        return !data.embedTitle || data.embedTitle.length > 256;
    });
    const embedDescriptionInvalid = derived(settings, (data) => {
        if (!data) return true;
        return !data.embedContent || data.embedContent.length > 4096;
    });
    const publicWebhookInvalid = derived(settings, (data) => {
        if (!data) return true;
        return data.auditLogWebhookURL && !webhookRegex.test(data.auditLogWebhookURL);
    });

    function reloadData() {
        API.get("/settings", CacheMode.API_ONLY, false)
            .then((res) => {
                settings.set({
                    ...res,
                    defaultLanguage: res.defaultLanguage.toString(),
                });
            })
            .catch(() => {
                toastError($_("admin.settings.failedtoload"));
            });
    }
    reloadData();

    function saveSettings() {
        submitting.set(true);
        API.put("/settings", $settings)
            .then((res) => {
                toastSuccess($_("admin.settings.saved"));
                settings.set({
                    ...res,
                    defaultLanguage: res.defaultLanguage.toString(),
                });
            })
            .catch(() => {
                toastError($_("admin.settings.failedtosave"));
            })
            .finally(() => {
                submitting.set(false);
            });
    }
</script>

<MediaQuery query="(min-width: 1024px)" let:matches>
    <div class="w-full lg:w-1/2">
        {#if $settings}
            <!-- embed settings -->
            <div class="text-2xl font-bold mb-4">
                {$_("admin.settings.title")}
            </div>
            <div class="text-xl font-semibold mb-2">
                {$_("admin.settings.embed.title")}
            </div>
            <div>
                {$_("admin.settings.embed.explained")}
            </div>
            <div class="mb-2">
                {$_("admin.settings.embed.explained2")}
            </div>
            <div>
                <TextInput
                    bind:value={$settings.embedTitle}
                    disabled={$submitting}
                    invalid={$embedTitleInvalid}
                    invalidText={$_("admin.settings.embed.titleinvalid")}
                    labelText={$_("admin.settings.embed.titlefield")} />
            </div>
            <div>
                <TextArea
                    bind:value={$settings.embedContent}
                    disabled={$submitting}
                    invalid={$embedDescriptionInvalid}
                    invalidText={$_("admin.settings.embed.descriptioninvalid")}
                    labelText={$_("admin.settings.embed.descriptionfield")} />
            </div>
            <div>
                <Checkbox bind:checked={$settings.embedShowIcon} disabled={$submitting} labelText={$_("admin.settings.embed.iconfield")} />
            </div>

            <!-- infrastructure settings -->

            <div class="text-xl font-semibold mt-6 mb-2">
                {$_("admin.settings.infrastructure.title")}
            </div>
            <div class="mb-2">
                {$_("admin.settings.infrastructure.explained")}
            </div>

            <div class="mb-4">
                <Select
                    disabled={$submitting}
                    labelText={$_("admin.settings.infrastructure.languagefield")}
                    bind:selected={$settings.defaultLanguage}>
                    {#each Languages.getAll() as language}
                        <SelectItem value={language.id.toString()} text={$_(language.translationKey)} />
                    {/each}
                </Select>
            </div>

            <div class="mb-2">
                {$_("admin.settings.infrastructure.explained2")}
            </div>

            <div class="mb-4">
                <TextInput
                    bind:value={$settings.auditLogWebhookURL}
                    disabled={$submitting}
                    invalid={$publicWebhookInvalid}
                    invalidText={$_("admin.settings.infrastructure.invalidwebhook")}
                    labelText={$_("admin.settings.infrastructure.webhookfield")} />
            </div>

            <div class="mb-2">
                {$_("admin.settings.infrastructure.explained3")}
            </div>

            <div class="mb-4">
                <Checkbox
                    bind:checked={$settings.publicFileMode}
                    disabled={$submitting}
                    labelText={$_("admin.settings.infrastructure.filemodefield")} />
            </div>

            <div class="flex flex-row justify-end">
                <Button
                    class="ml-auto"
                    on:click={saveSettings}
                    disabled={$submitting || $embedTitleInvalid || $embedDescriptionInvalid || $publicWebhookInvalid}>
                    {$_("admin.settings.save")}
                </Button>
            </div>
        {:else}
            <!-- embed settings -->
            <div class="mb-4">
                <SkeletonText heading />
            </div>
            <div class="mb-2">
                <SkeletonText heading />
            </div>
            <div class="mb-2">
                <SkeletonText paragraph rows={2} />
            </div>
            <div class="mb-1">
                <TextInputSkeleton />
            </div>
            <div>
                <TextInputSkeleton />
            </div>
            <div>
                <Checkbox skeleton />
            </div>

            <!-- infrastructure settings -->

            <div class="mt-6 mb-2">
                <SkeletonText heading />
            </div>
            <div class="mb-2">
                <SkeletonText paragraph rows={2} />
            </div>

            <div class="mb-4">
                <SelectSkeleton />
            </div>

            <div class="mb-2">
                <SkeletonText paragraph rows={2} />
            </div>

            <div class="mb-4">
                <TextInputSkeleton />
            </div>

            <div class="mb-2">
                <SkeletonText paragraph rows={2} />
            </div>

            <div class="mb-4">
                <Checkbox skeleton />
            </div>

            <div class="flex flex-row justify-end">
                <Button skeleton />
            </div>
        {/if}
    </div>
</MediaQuery>
