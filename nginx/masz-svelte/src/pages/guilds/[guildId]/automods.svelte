<script lang="ts">
    import { currentFlatpickrLocale, currentLanguage } from "./../../../stores/currentLanguage";
    import { DatePicker, DatePickerInput, TimePicker } from "carbon-components-svelte";
    import { _ } from "svelte-i18n";
    import type { ILanguageSelect } from "../../../models/ILanguageSelect";
    import moment from "moment";

    const utfOffset = new Date().getTimezoneOffset() * -1;
    let inputPunishedUntilDate: any;
    let inputPunishedUntilTime: any;
    let punishedUntil;

    $: calculatePunishedUntil(inputPunishedUntilDate, inputPunishedUntilTime, $currentLanguage);
    function calculatePunishedUntil(date: string, time: string, language?: ILanguageSelect) {
        if (language && date) {
            punishedUntil = date
                ? moment(`${date} ${time ? time : "00:00"}`, `${language.momentDateFormat} ${language.momentTimeFormat}`)
                      .utc(false)
                      .utcOffset(utfOffset)
                : null;
        } else {
            punishedUntil = null;
        }
    }
</script>

<div class="flex flex-row">
    <DatePicker
        class="!grow-0 !shrink mr-4"
        bind:value={inputPunishedUntilDate}
        datePickerType="single"
        locale={$currentFlatpickrLocale ?? "en"}
        dateFormat={$currentLanguage?.dateFormat}
        on:change>
        <DatePickerInput labelText={$_("guilds.casedialog.punisheduntil")} placeholder={$currentLanguage?.dateFormat} />
    </DatePicker>
    <TimePicker
        class="!grow-0"
        bind:value={inputPunishedUntilTime}
        invalid={!!inputPunishedUntilTime && !/(1[012]|[1-9]):[0-5][0-9](\\s)?/.test(inputPunishedUntilTime)}
        invalidText={$_("guilds.casedialog.formatisrequired", { values: { format: $currentLanguage?.timeFormat } })}
        labelText={$_("guilds.casedialog.punisheduntil")}
        placeholder={$currentLanguage?.timeFormat} />
</div>

<div class="flex flex-col">
    <div>
        date: {inputPunishedUntilDate}
    </div>
    <div>
        time: {inputPunishedUntilTime}
    </div>
    <div>
        offset: {utfOffset}
    </div>
    <div>
        until: {punishedUntil?.toISOString()}
    </div>
</div>
