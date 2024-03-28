<script>
    // @ts-nocheck
    import { createEventDispatcher, onMount, tick } from "svelte";

    const dispatch = createEventDispatcher();

    /** @type {number} */
    export let ratio;

    /** @type {number} */
    export let maxWidth;

    export let imgFileName = 'mandelbrot.png';

    /** @type {boolean} */
    export let loading = false;

    /** @type {HTMLElement} */
    let container;

    let dragging = false;
    let offsetX = 0;
    let offsetY = 0;
    let currentX = 0;
    let currentY = 0;
    let sizeW = 200;
    
    $: sizeH = Math.floor(sizeW * ratio);

    $: imgUrl = `http://localhost:3333/${imgFileName}`;

    /** @type {number} */
    let width;
    /** @type {number} */
    let height;

    onMount(async () => {
        await recalculateSizes();
    });

    async function handleResize() {
        await recalculateSizes();
    }

    async function recalculateSizes() {
        width = container.clientWidth - 10;
        height = Math.floor(width * ratio);
        container.style.height = `${height}px`;
        sizeW = Math.min(sizeW, width);
        await tick();
        onChange();
    }

    /** @param {MouseEvent} event */
    function startDrag(event) {
        dragging = true;
        offsetX = event.clientX - currentX;
        offsetY = event.clientY - currentY;
    }

    function endDrag() {
        if (dragging) {
            onChange();
        }
        dragging = false;
    }

    /**
     * @param {MouseEvent} event
     */
    function drag(event) {
        if (!dragging) return;
        const newX = event.clientX - offsetX;
        const newY = event.clientY - offsetY;
        
        currentX = Math.max(0, Math.min(newX, width - sizeW - 2));
        currentY = Math.max(0, Math.min(newY, height - sizeH - 2));
    }

    /** @param {WheelEvent} event */
    async function wheel(event) {
        event.preventDefault();

        const centerX = Math.floor(sizeW / 2 + currentX);
        const centerY = Math.floor(sizeH / 2 + currentY);

        if (event.deltaY > 0) {
            sizeW = Math.max(100, Math.floor(sizeW * 0.9));
            await tick();
            currentX = Math.floor(centerX - sizeW / 2);
            currentY = Math.floor(centerY - sizeH / 2);
        } else {
            sizeW = Math.min(width, Math.floor(sizeW * 1.1));
            await tick();
            currentX = Math.floor(centerX - sizeW / 2);
            currentY = Math.floor(centerY - sizeH / 2);

            currentX = currentX < 0 ? 0 : currentX;
            currentY = currentY < 0 ? 0 : currentY;

            currentX = Math.max(0, Math.min(currentX, width - sizeW - 2));
            currentY = Math.max(0, Math.min(currentY, height - sizeH - 2));
        }
        onChange();
    }

    function onChange() {
        dispatch('change', {
            x: currentX,
            y: currentY,
            width: sizeW,
            height: sizeH,
            maxW: width,
            maxH: height
        });
    }
</script>

<svelte:window on:resize={handleResize}></svelte:window>
<svelte:document on:mouseup={endDrag}></svelte:document>

<!-- svelte-ignore a11y-no-static-element-interactions -->
<div class="container" on:mousemove={drag} bind:this={container} style="max-width: {maxWidth}px">
    <!-- svelte-ignore a11y-no-static-element-interactions -->
    <img src={imgUrl} alt="">
    {#if !loading}
        <div class="frame" style="left: {currentX}px; top: {currentY}px; width: {sizeW}px; height: {sizeH}px;" on:mousedown={startDrag} on:mousewheel={wheel}>
            <div class="frame0">
                <div class="frame1"></div>
            </div>
        </div>
    {:else}
        <div class="loading-background">

        </div>
    {/if}
</div>

<style lang="scss">
    @import "../../styles/hm.scss";

    .container {
        box-sizing: content-box;
        width: 100%;
        position: relative;
        border: 1px solid $gray-700;

        img {
            width: 100%;
            height: 100%;
        }

        .frame {
            position: absolute;
            z-index: 10;
            border: 1px solid blue;
            cursor: move;

            .frame0 {
                border: 1px solid #fff;
                height: 100%;
            }

            .frame1 {
                border: 1px solid #adf048;
                height: 100%;
            }
        }
    }

    .loading-background {
        z-index: 100;
        background-color: #fff;
        opacity: 0.4;
        position: absolute;
        left: 0;
        top: 0;
        bottom: 0;
        right: 0;

        background: linear-gradient(90deg, #350303, #fff, #350303);
        background-size: 200% 200%;
        animation: iridescent 4s linear infinite;
    }

    @keyframes iridescent {
        0% {
            background-position: 0% 50%;
        }
        50% {
            background-position: 100% 50%;
        }
        100% {
            background-position: 0% 50%;
        }
    }
</style>