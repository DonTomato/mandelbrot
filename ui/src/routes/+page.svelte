<script>
    import SelectorPage from "$lib/pages/SelectorPage.svelte";
    import { currentFrame } from "$lib/store";

    /** @type {any} */
    export let data;

    currentFrame.set(data);
</script>

<h1>Observer</h1>

<SelectorPage />