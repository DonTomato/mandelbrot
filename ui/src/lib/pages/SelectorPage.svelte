<script>
    import Cropper from "$lib/components/Cropper.svelte";
    import { baseParameters, currentFrame, loading, userId } from "$lib/store";
    import ParametersView from "./ParametersView.svelte";

    let pictureSize = { width: 0, height: 0 };
    let zoomFrame = { 
        width: 0, 
        height: 0,
        x: 0,
        y: 0
    };

    $: di = getDeepIndex($currentFrame.w);

    $: parameters = [
        { key: 'Width', value: pictureSize.width },
        { key: 'Height', value: pictureSize.height },
        { key: 'Frame X', value: zoomFrame.x },
        { key: 'Frame Y', value: zoomFrame.y },
        { key: 'Frame Width', value: zoomFrame.width },
        { key: 'Frame Height', value: zoomFrame.height },
        { key: 'DI', value: di }
    ];

    /** @param {CustomEvent} event */
    function handleChange(event) {
        pictureSize = {
            width: event.detail.maxW,
            height: event.detail.maxH
        };
        zoomFrame = {
            x: event.detail.x,
            y: event.detail.y,
            width: event.detail.width,
            height: event.detail.height
        };
    }

    async function zoomIn() {
        const currentData = $currentFrame;
        const request = {
            user_id: $userId,
            w: currentData.w,
            x: currentData.x,
            y: currentData.y,
            width: pictureSize.width,
            height: pictureSize.height,
            frame_x: zoomFrame.x,
            frame_y: zoomFrame.y,
            frame_w: zoomFrame.width
        };

        const apiBasePath = import.meta.env.VITE_API_BASE_PATH;
	    const response = await fetch(`${apiBasePath}/generate`, { 
            method: 'POST', 
            headers: {
                'Accept': 'application/json, text/plain, */*',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(request)     
        });

        $loading = true;
    }

    async function zoomOut() {
        const currentData = $currentFrame;
        const request = {
            user_id: $userId,
            w: currentData.w,
            x: currentData.x,
            y: currentData.y,
            width: pictureSize.width,
            height: pictureSize.height,
            frame_x: -Math.floor(pictureSize.width / 4),
            frame_y: -Math.floor(pictureSize.height / 4),
            frame_w: pictureSize.width * 2
        };

        const apiBasePath = import.meta.env.VITE_API_BASE_PATH;
	    await fetch(`${apiBasePath}/generate`, { 
            method: 'POST', 
            headers: {
                'Accept': 'application/json, text/plain, */*',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(request)     
        });
        $loading = true;
    }

    /** @param {number} w */
    function getDeepIndex(w) {
        let _w = w;
        let index = 0;
        if (w > 1) {
            return 0;
        } else {
            while (_w < 1) {
                _w = _w *= 10;
                index++;
            }
            return index;
        }
    }
</script>

<div class="page-container">
    <div class="image-container">
        <Cropper 
            ratio={$baseParameters.ratio} 
            maxWidth={$baseParameters.w}
            imgFileName={$currentFrame.fileName}
            loading={$loading}
            on:change={handleChange} />
    </div>

    <div class="image-info">
        <ParametersView {parameters} />

        <div class="toolbar">
            <button class="mnd-button" on:click={zoomIn}>Zoom In</button>
            <button class="mnd-button" on:click={zoomOut}>Zoom Out</button>
        </div>

        <div style="margin-top: 2rem;">
            <span>Loading: <span class="mdb-mono">{$loading}</span></span>
        </div>
    </div>
</div>


<style lang="scss">
    @import "../../styles/hm.scss";
    
    .page-container {
        display: flex;
        gap: 2rem;
    }

    .image-container {
        flex: 1 0 0;
    }

    .image-info {
        width: 30rem;
    }

    .toolbar {
        margin-top: 2rem;
        display: flex;
        gap: 1rem;
    }
</style>