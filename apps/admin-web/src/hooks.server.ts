import type { Handle, HandleFetch } from '@sveltejs/kit';

export const handle: Handle = async ({ event, resolve }) => {
    const token = event.cookies.get('token');

    if (token) {
        // Decode token minimally to get basic info if needed, or just pass it to gateway.
        event.locals.user = { isAuthenticated: true, token };
    } else {
        event.locals.user = { isAuthenticated: false };
    }

    const response = await resolve(event);
    return response;
};

export const handleFetch: HandleFetch = async ({ request, fetch, event }) => {
    // If request is going to our internal Gateway (docker network or localhost port)
    // We attach the token.
    // In Production/Docker, Gateway is at http://gateway:8080 or similar.
    // Client-side fetch goes to same domain (BFF proxy) or Gateway URL.

    // For Server-Side Load functions:
    if (request.url.startsWith('http://localhost:8080') || request.url.startsWith('http://gateway:8080')) {
        if (event.locals.user?.token) {
            request.headers.set('Authorization', `Bearer ${event.locals.user.token}`);
        }
    }

    return fetch(request);
};
