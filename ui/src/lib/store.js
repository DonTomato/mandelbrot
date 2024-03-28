import { readable, writable } from "svelte/store";
import { v4 as uuidv4 } from 'uuid';

export const baseParameters = writable({
    w: 10,
    h: 10,
    ratio: 1
});

export const currentFrame = writable({
    x: -0.7,
    y: 0,
    w: 3.5,
    width: 1000,
    height: 750,
    fileName: undefined
});

export const loading = writable(false);

export const userId = readable(uuidv4().replaceAll('-', ''));
