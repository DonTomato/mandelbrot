import { baseParameters, userId, loading } from '$lib/store';

/** @type {import('./$types').PageLoad} */
export async function load({ fetch }) {
    let id;
    userId.subscribe(e => id = e);
    const apiBasePath = import.meta.env.VITE_API_BASE_PATH;
	const response = await fetch(`${apiBasePath}/gen/framesize/${id}`);
    const data = await response.json();
    baseParameters.set({
        w: data.width,
        h: data.height,
        ratio: data.height / data.width
    });
    loading.set(true);
    // return data;
}
