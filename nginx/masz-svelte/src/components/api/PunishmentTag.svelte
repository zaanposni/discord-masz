<script lang="ts">
    import { Tag } from "carbon-components-svelte";
    import { _ } from "svelte-i18n";
    import { getI18NPunishment, ICase } from "../../models/api/ICase";
    import { PunishmentType } from "../../models/api/PunishmentType";

    let classes: string = "";
    export { classes as class };
    export let modCase: ICase;
    let type: string;

    function selectType() {
        if (!modCase.punishmentActive) {
            type = "gray";
            return;
        }
        if (modCase.punishmentType === PunishmentType.Ban) {
            type = "red";
            return;
        } else if (modCase.punishmentType === PunishmentType.Mute) {
            type = "purple";
            return;
        } else if (modCase.punishmentType === PunishmentType.Kick) {
            type = "teal";
            return;
        }
        type = "purple";
    }
    selectType();
</script>

<Tag size="default" {type} class={classes}>
    {$_(getI18NPunishment(modCase))}
</Tag>
