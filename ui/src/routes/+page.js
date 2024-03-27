import { baseParameters } from '$lib/store';

/** @type {import('./$types').PageLoad} */
export async function load({ fetch }) {
    const apiBasePath = import.meta.env.VITE_API_BASE_PATH;
	const response = await fetch(`${apiBasePath}/gen`);
    // const data = await response.json();
    // baseParameters.set({
    //     w: data.width,
    //     h: data.height,
    //     ratio: data.height / data.width
    // });
    // return data;
}
