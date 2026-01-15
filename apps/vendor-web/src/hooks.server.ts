import type { Handle, HandleFetch } from '@sveltejs/kit';

export const handle: Handle = async ({ event, resolve }) => {
    const token = event.cookies.get('token');

    if (token) {
        event.locals.user = { isAuthenticated: true, token };
    } else {
        event.locals.user = { isAuthenticated: false };
    }

    const response = await resolve(event);
    return response;
};

export const handleFetch: HandleFetch = async ({ request, fetch, event }) => {
    if (request.url.startsWith('http://localhost:8080') || request.url.startsWith('http://gateway:8080')) {
        if (event.locals.user?.token) {
            request.headers.set('Authorization', `Bearer ${event.locals.user.token}`);
        }
    }

    return fetch(request);
};
