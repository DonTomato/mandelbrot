<script>
    import { onMount } from "svelte";
    import Header from "$lib/components/Header.svelte";
    import "../styles/main.scss";
    import { currentFrame, loading, userId } from "$lib/store";
    import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

    /** @type {HubConnection} */
    let connection;

    onMount(async () => {
        await initSingnalR();
    });

    async function initSingnalR() {
        const apiBasePath = import.meta.env.VITE_API_BASE_PATH;
        connection = new HubConnectionBuilder()
            .withUrl(`${apiBasePath}/ws?userId=${$userId}`)
            .withAutomaticReconnect()
            .build();

        try {
            await connection.start();
            console.log('SignalR connection started', connection.connectionId);

            connection.on('FrameCreated', (e) => {
                handleFrame(JSON.parse(e));
            });
        } catch (err) {
            console.error('Error while starting connection: ' + err);
        }
    }

    /**
     * @param frame {any}
     */
    function handleFrame(frame) {
        $currentFrame.w = frame.w;
        $currentFrame.x = frame.x;
        $currentFrame.y = frame.y;
        $currentFrame.fileName = frame.fileName;

        $loading = false;
    }
</script>

<svelte:head>
    <title>Mandelbrot Set</title>
</svelte:head>

<div class="mnd-content-container">
    <Header />
    
    <slot />
</div>