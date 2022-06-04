<script lang="ts">
    import { Tag } from "carbon-components-svelte";
    import { _ } from "svelte-i18n";
    import { getI18NPunishment, ICase } from "../../models/api/ICase";
    import { PunishmentType } from "../../models/api/PunishmentType";

    let classes: string = "";
    export let defaultColor: string = "gray";
    export { classes as class };
    export let modCase: ICase;
    let type: string;

    function selectType(m: ICase) {
        if (!modCase.punishmentActive) {
            type = defaultColor;
            return;
        }
        if (modCase.punishmentType === PunishmentType.Ban) {
            type = "red";
            return;
        } else if (modCase.punishmentType === PunishmentType.Mute) {
            type = "magenta";
            return;
        } else if (modCase.punishmentType === PunishmentType.Kick) {
            type = "purple";
            return;
        }
        type = "purple";
    }
    $: selectType(modCase);
</script>

<Tag size="default" {type} class={classes}>
    {$_(getI18NPunishment(modCase))}
</Tag>
