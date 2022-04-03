<script lang="ts">
    import type { ILicenseInfo } from "./ILicenseInfo";
    import { Modal, OutboundLink } from "carbon-components-svelte";
    import API from "../../../services/api/api";
    import { showCredits } from "./store";
    import { Accordion, AccordionItem } from "carbon-components-svelte";
import { APP_VERSION } from "../../../config";

    function onModalClose() {
        showCredits.set(false);
    }

    let licenses: ILicenseInfo = { python: new Map(), dotnet: new Map(), npm: new Map() };

    API.getStatic("/static/licenses.json").then((data: ILicenseInfo) => {
        licenses = data;
    });
</script>

<Modal size="sm" open={$showCredits} selectorPrimaryFocus="#maszbyzaanposni" modalHeading="Credits" passiveModal on:close={onModalClose} on:submit={onModalClose}>
    <div id="maszbyzaanposni">MASZ by zaanposni</div>
    <div>Current version: {APP_VERSION}</div>
    <OutboundLink href="https://github.com/zaanposni/discord-masz">GitHub</OutboundLink>

    <Accordion class="mt-4">
        <AccordionItem title="npm dependencies">
            <ul>
                {#each Object.entries(licenses.npm) as [key, value]}
                    <li>
                        {key}: {value.licenses} {value.publisher ? `by ${value.publisher}` : ''}
                    </li>
                {/each}
            </ul>
        </AccordionItem>
        <AccordionItem title="python dependencies">
            <ul>
                {#each Object.entries(licenses.python) as [key, value]}
                    <li>
                        {key}: {value.licenses} {value.publisher ? `by ${value.publisher}` : ''}
                    </li>
                {/each}
            </ul>
        </AccordionItem>
        <AccordionItem title="dotnet dependencies">
            <ul>
                {#each Object.entries(licenses.dotnet) as [key, value]}
                    <li>
                        {key}: {value.licenses} {value.publisher ? `by ${value.publisher}` : ''}
                    </li>
                {/each}
            </ul>
        </AccordionItem>
    </Accordion>
</Modal>
