<script lang="ts">
    import { Tag } from "carbon-components-svelte";
    import { _ } from "svelte-i18n";
    import { getI18NStatus } from "../../models/api/IAppealView";
    import type { IAppealView } from "../../models/api/IAppealView";
    import { PunishmentType } from "../../models/api/PunishmentType";
    import { AppealStatus } from "../../models/api/AppealStatus";

    let classes: string = "";
    export { classes as class };
    export let appeal: IAppealView;
    let type: string;

    function selectType(status: AppealStatus) {
        switch (status) {
            case AppealStatus.Approved:
                type = "green";
                break;
            case AppealStatus.Denied:
                type = "red";
                break;
            default:
                type = "cyan";
                break;
        }
    }
    $: appeal != null ? selectType(+appeal.status) : undefined;
</script>

<Tag size="default" {type} class={classes}>
    {$_(getI18NStatus(appeal))}
</Tag>
