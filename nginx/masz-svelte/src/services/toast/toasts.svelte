<script lang="ts">
  import { ToastNotification } from "carbon-components-svelte";
  import { dismissToast, toasts, setToastHovered } from "./store";
</script>

{#if $toasts}
  <section>
    {#each $toasts as toast}
      <ToastNotification
        kind={toast.type}
        title={toast.title}
        subtitle={toast.message}
        caption={new Date().toLocaleString()}
        on:close={() => dismissToast(toast.id)}
        on:mouseover={() => setToastHovered(toast.id, true)}
        on:mouseleave={() => setToastHovered(toast.id, false)}
      />
    {/each}
  </section>
{/if}

<style lang="postcss">
  section {
    position: fixed;
    top: 30px;
    right: 10px;
    display: flex;
    margin-top: 1rem;
    justify-content: center;
    align-items: flex-end;
    flex-direction: column;
    z-index: 1000;
  }
</style>
