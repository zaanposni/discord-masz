<script lang="ts">
    import { Tag } from "carbon-components-svelte";
    import { _ } from "svelte-i18n";
    import { AutomodAction } from "../../models/api/AutomodActionEnum";
    import automodActions from "../../services/enums/AutomoderationAction";

    let classes: string = "";
    export { classes as class };
    export let action: AutomodAction;
    let type: string;

    function selectType(m: AutomodAction) {
        switch (action) {
            case AutomodAction.CaseCreated:
                type = "cyan";
                break;
            case AutomodAction.ContentDeletedAndCaseCreated:
                type = "blue";
                break;
            case AutomodAction.ContentDeleted:
                type = "purple";
                break;
            case AutomodAction.Timeout:
                type = "green";
                break;
            default:
                type = "cool-gray";
        }
    }
    $: selectType(+action);
</script>

<Tag size="default" {type} class={classes}>
    {$_(automodActions.getById(action))}
</Tag>
