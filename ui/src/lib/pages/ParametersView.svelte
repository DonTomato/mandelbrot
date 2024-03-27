<script>

    /**
     * @typedef {Object} ParameterItem
     * @property {string} key
     * @property {number} value
     */

    /** @type {ParameterItem[]} */
    export let parameters = [];
</script>

<div class="container">
    {#each parameters as param}
        <div class="item">
            <div class="key">{param.key}</div>
            <div class="value mdb-mono">{param.value}</div>
        </div>
    {/each}
</div>

<style lang="scss">
    .container {
        display: flex;
        flex-direction: column;
        gap: 1rem;

        .item {
            display: flex;

            .key {
                flex: 1 0 0;
            }

            .value {
                flex: 1 0 0;
            }
        }
    }
</style>