<script>
    import { onMount } from "svelte";
    import Header from "$lib/components/Header.svelte";
    import "../styles/main.scss";
    import { currentFrame, loading, userId } from "$lib/store";
    import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

    const wsUrl = 'ws://localhost:3000/ws';

    /** @type {HubConnection} */
    let connection;

    onMount(async () => {
        await initSingnalR();
    });

    async function initSingnalR() {
        const apiBasePath = import.meta.env.VITE_API_BASE_PATH;
        connection = new HubConnectionBuilder()
            .withUrl(`${apiBasePath}/ws`)
            .build();

        try {
            await connection.start();
            console.log('SignalR connection started', connection.connectionId);

            connection.on('Initial', (e) => {
                console.log('WS SR message received', e);
            });

            connection.invoke('RegisterClient', $userId)
                .then(_ => {
                    $loading = true;
                    console.log('Client is registered');
                })
                .catch(e => console.error('WS SR: Register User error', e));
        } catch (err) {
            console.error('Error while starting connection: ' + err);
        }
    }
</script>

<svelte:head>
    <title>Mandelbrot Set</title>
</svelte:head>

<div class="mnd-content-container">
    <Header />
    
    <slot />
</div>