<script>
    import { DatePicker, DatePickerInput, TimePicker } from "carbon-components-svelte";

    let value;

    $: console.log(value);
</script>

<div class="flex flex-row">
    <DatePicker class="grow-0" datePickerType="single" bind:value>
        <DatePickerInput labelText="Meeting date" placeholder="mm/dd/yyyy" />
    </DatePicker>
    <TimePicker labelText="Cron job" placeholder="hh:mm" />
</div>

{value}
