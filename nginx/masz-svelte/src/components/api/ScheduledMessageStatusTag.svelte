<script lang="ts">
    import { Tag } from "carbon-components-svelte";
    import { _ } from "svelte-i18n";
    import { getI18NMessageStatus, IScheduledMessage } from "../../models/api/IScheduledMessage";
    import { ScheduledMessageStatus } from "../../models/api/ScheduledMessageStatus";

    let classes: string = "";
    export { classes as class };
    export let message: IScheduledMessage;
    let type: string;

    function selectType(m: IScheduledMessage) {
        switch (message.status) {
            case ScheduledMessageStatus.Sent:
                type = "green";
                break;
            case ScheduledMessageStatus.Failed:
                type = "red";
                break;
            default:
                type = "cyan";
        }
    }
    $: selectType(message);
</script>

<Tag size="default" {type} class={classes}>
    {$_(getI18NMessageStatus(message))}
</Tag>
