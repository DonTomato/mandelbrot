<script>
    import { onMount } from "svelte";
    import Header from "$lib/components/Header.svelte";
    import "../styles/main.scss";
    import { currentFrame, loading, userId } from "$lib/store";
    import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

    const wsUrl = 'ws://localhost:3000/ws';

    /** @type {WebSocket} */
    let socket;

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

            connection.on('set_created', (e) => {
                console.log('WS SR message received', e);
            });

            connection.invoke('RegisterClient', $userId)
                .catch(e => console.error('WS SR: Register User error', e));
        } catch (err) {
            console.error('Error while starting connection: ' + err);
        }
    }

    function initializeSocket() {
        if (socket) {
            try {
                socket.close();
            } catch {}
        }

        socket = new WebSocket(wsUrl);

        // Connection opened
        socket.addEventListener('open', (event) => {
            console.log('WebSocket connection opened');
            const connect = {
                user_id: $userId
            };
            socket.send(JSON.stringify(connect));
        });

        // Listen for messages
        socket.addEventListener('message', async (event) => {
            try {
                const responseBody = JSON.parse(event.data);
                
                $currentFrame.w = responseBody.w;
                $currentFrame.x = responseBody.x;
                $currentFrame.y = responseBody.y;
                $currentFrame.fileName = responseBody.file_name;
                $loading = false;
            } catch (err) {
                console.error('ERROR Message', event.data);
            }
        });

        // Connection closed
        socket.addEventListener('close', (event) => {
            console.warn('WebSocket connection closed');
            setTimeout(() => {
                // initializeSocket();
            }, 2000);
        });

        // Error handling
        socket.addEventListener('error', (event) => {
            console.error('WebSocket error:', event);
            // initializeSocket();
        });
    }
</script>

<svelte:head>
    <title>Mandelbrot Set</title>
</svelte:head>

<div class="mnd-content-container">
    <Header />
    
    <slot />
</div>