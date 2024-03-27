<script>
    import { userId } from "$lib/store";
</script>

<div class="header-container">
    <div class="left">
        <div class="brand">
            <a href="/" class="mnd-link">Mandelbrot</a>
        </div>
    
        <a href="/" class="mnd-link">Home</a>
    </div>

    <div class="right">
        <span class="mdb-mono">{$userId}</span>
    </div>
</div>

<style lang="scss">
    @import "src/styles/hm.scss";

    .header-container {
        display: flex;
        justify-content: space-between;
        align-items: center;
        height: 6rem;

        > * {
            display: flex;
            align-items: center;
        }

        > a {
            margin-right: 2rem;
        }
    }

    .brand {
        font-size: 2rem;
        margin-right: 4rem;

        a {
            text-decoration: none;

            &:hover {
                text-decoration: underline;
            }
        }
    }
</style>