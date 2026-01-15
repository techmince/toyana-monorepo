import { redirect } from '@sveltejs/kit';

export const load = async ({ fetch, locals }) => {
    if (!locals.user?.token) throw redirect(303, '/login');

    // Fetch Services
    const res = await fetch('http://localhost:8080/vendors/services', {
        headers: {
            'Authorization': `Bearer ${locals.user.token}`
        }
    });

    if (!res.ok) {
        return { services: [] };
    }

    const services = await res.json();
    return { services };
};
