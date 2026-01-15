import { fail, redirect } from '@sveltejs/kit';
import { superValidate } from 'sveltekit-superforms';
import { zod } from 'sveltekit-superforms/adapters';
import { serviceSchema } from '$lib/schemas/services';

export const load = async () => {
    const form = await superValidate(zod(serviceSchema));
    return { form };
};

export const actions = {
    default: async ({ request, fetch, locals }) => {
        if (!locals.user?.token) return fail(401);

        const form = await superValidate(request, zod(serviceSchema));
        if (!form.valid) return fail(400, { form });

        try {
            const res = await fetch('http://localhost:8080/vendors/services', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${locals.user.token}`
                },
                body: JSON.stringify(form.data)
            });

            if (!res.ok) {
                const err = await res.json();
                return fail(res.status, { form, error: err.detail || 'Failed to add service' });
            }
        } catch (e) {
            return fail(500, { form, error: 'Network error' });
        }

        throw redirect(303, '/services');
    }
};
