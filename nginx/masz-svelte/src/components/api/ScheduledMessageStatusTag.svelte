<script lang="ts">
    import { Tag } from "carbon-components-svelte";
    import { _ } from "svelte-i18n";
    import { getI18NPunishment, IScheduledMessage } from "../../models/api/IScheduledMessage";
    import { ScheduledMessageStatus } from "../../models/api/ScheduledMessageStatus";

    let classes: string = "";
    export { classes as class };
    export let message: IScheduledMessage;
    let type: string;

    function selectType() {
        switch (message.status) {
            case ScheduledMessageStatus.Sent:
                type = "green";
            case ScheduledMessageStatus.Failed:
                type = "red";
            default:
                type = "cyan";
        }
    }
    selectType();
</script>

<Tag size="default" {type} class={classes}>
    {$_(getI18NPunishment(message))}
</Tag>
