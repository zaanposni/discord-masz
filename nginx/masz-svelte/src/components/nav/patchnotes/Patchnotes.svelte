<script lang="ts">
    import { OutboundLink } from "carbon-components-svelte";
    import { CheckmarkFilled24, CircleSolid24, LogoGithub24, StarFilled24, WarningFilled24 } from "carbon-icons-svelte";
    import moment from "moment";
    import type { Writable } from "svelte/store";
    import { writable } from "svelte/store";
    import MediaQuery from "../../../core/MediaQuery.svelte";
    import type { IPatchnote, IPatchnoteSeperator } from "../../../models/IPatchnote";
    import API from "../../../services/api/api";
    import { currentLanguage } from "../../../stores/currentLanguage";

    const patchnotes: Writable<Array<IPatchnote | IPatchnoteSeperator>> = writable([]);

    API.getStatic("/static/patchnotes.json").then((data: IPatchnote[]) => {
        const res = [{ text: "More coming..." }];
        for (let i = 0; i < data.length; i++) {
            res.push(data[i]);
            if (i + 1 < data.length - 1) {
                // calculate diff in days using moment
                if (data[i + 1].released_at) {
                    const diff = moment(data[i].released_at).diff(moment(data[i + 1].released_at), "days");
                    if (diff) {
                        res.push({
                            text: `${diff} days`,
                        });
                    }
                }
            }
        }
        patchnotes.set(res);
    });
</script>

<style>
    .contributors:hover > a {
        margin-right: 0.2rem;
    }
    .contributors > a {
        transform: scale(1);
        transition: 0.5s all ease;
    }
</style>

<MediaQuery query="(min-width: 1024px)" let:matches>
    <div class="flex flex-row items-center justify-center w-full">
        <div class="grid grid-cols-8 gap-0 w-full lg:w-1/2">
            {#each $patchnotes as patchnote}
                {#if patchnote.text}
                    {#if matches}
                        <div />
                    {/if}
                    <div
                        class="flex flex-row justify-center border-solid border-2 col-span-2 px-1 py-1" class:mx-4={matches}
                        style="border-color: #161616; color: var(--cds-text-05, #6f6f6f)">
                        <div class="">
                            {patchnote.text}
                        </div>
                    </div>
                    <div class="col-span-5" class:col-span-6={!matches} />
                {:else}
                    {#if matches}
                        <div
                            class="col-span-2 pb-8 pr-4"
                            style="color: var(--cds-text-05, #6f6f6f); padding-top: 2.6rem">
                            {patchnote.released_at != undefined
                                ? moment(patchnote.released_at).format($currentLanguage?.momentDateFormat ?? "DD/MM/YYYY")
                                : "Unknown"}
                        </div>
                    {:else}
                        <div />
                    {/if}
                    <div class="border-solid border-l-2 py-8 pl-4 col-span-6" class:col-span-7={!matches} style="border-color: #161616">
                        <div class="text-xl font-bold mb-4">{patchnote.version}</div>
                        {#if patchnote.contributors}
                            <div class="flex flex-row flex-wrap mb-4 contributors">
                                {#each patchnote.contributors as contributor}
                                    <a class="-mr-6" href={contributor.link} target="_blank">
                                        <img class="w-12 h-12 rounded-full" src={contributor.icon} alt={contributor.name} />
                                    </a>
                                {/each}
                            </div>
                        {/if}
                        <div class="grid grid-cols-8 gap-x-0 gap-y-4">
                            {#each patchnote?.breaking ?? [] as entry}
                                <div class="flex flex-row col-span-7">
                                    <div>
                                        <WarningFilled24 style="fill: var(--cds-support-error-inverse)" />
                                    </div>
                                    <div class="flex flex-col px-4">
                                        <div class="text-lg font-semibold">{entry.title}</div>
                                        <div>{entry.description}</div>
                                    </div>
                                </div>
                                <div class="pt-1">
                                    {#if entry.link}
                                        <OutboundLink href={entry.link}>Link</OutboundLink>
                                    {/if}
                                </div>
                            {/each}
                            {#each patchnote?.features ?? [] as entry}
                                <div class="flex flex-row col-span-7">
                                    <div>
                                        <StarFilled24 style="fill: var(--cds-support-warning-inverse)" />
                                    </div>
                                    <div class="flex flex-col px-4">
                                        <div class="text-lg font-semibold">{entry.title}</div>
                                        <div>{entry.description}</div>
                                    </div>
                                </div>
                                <div class="pt-1">
                                    {#if entry.link}
                                        <OutboundLink href={entry.link}>Link</OutboundLink>
                                    {/if}
                                </div>
                            {/each}
                            {#each patchnote?.fixes ?? [] as entry}
                                <div class="flex flex-row col-span-7">
                                    <div>
                                        <CheckmarkFilled24 style="fill: var(--cds-support-success-inverse)" />
                                    </div>
                                    <div class="flex flex-col px-4">
                                        <div class="text-lg font-semibold">{entry.title}</div>
                                        <div>{entry.description}</div>
                                    </div>
                                </div>
                                <div class="pt-1">
                                    {#if entry.link}
                                        <OutboundLink href={entry.link}>Link</OutboundLink>
                                    {/if}
                                </div>
                            {/each}
                            {#each patchnote?.technical ?? [] as entry}
                                <div class="flex flex-row col-span-7">
                                    <div>
                                        <LogoGithub24 style="fill: var(--cds-support-info)" />
                                    </div>
                                    <div class="flex flex-col px-4">
                                        <div class="text-lg font-semibold">{entry.title}</div>
                                        <div>{entry.description}</div>
                                    </div>
                                </div>
                                <div class="pt-1">
                                    {#if entry.link}
                                        <OutboundLink href={entry.link}>Link</OutboundLink>
                                    {/if}
                                </div>
                            {/each}
                            {#each patchnote?.changes ?? [] as entry}
                                <div class="flex flex-row col-span-7">
                                    <div>
                                        <CircleSolid24 style="fill: #161616" />
                                    </div>
                                    <div class="flex flex-col px-4">
                                        <div>{entry}</div>
                                    </div>
                                </div>
                                <div class="pt-1">
                                    {#if entry.link}
                                        <OutboundLink href={entry.link}>Link</OutboundLink>
                                    {/if}
                                </div>
                            {/each}
                        </div>
                    </div>
                {/if}
            {/each}
        </div>
    </div>
</MediaQuery>
