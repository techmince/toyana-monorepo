import { fail, redirect } from '@sveltejs/kit';
import { superValidate } from 'sveltekit-superforms';
import { zod } from 'sveltekit-superforms/adapters';
import { registerSchema } from '$lib/schemas/auth';

export const load = async () => {
    const form = await superValidate(zod(registerSchema));
    return { form };
};

export const actions = {
    default: async ({ request, fetch, cookies }) => {
        const form = await superValidate(request, zod(registerSchema));

        if (!form.valid) {
            return fail(400, { form });
        }

        try {
            // Call Gateway
            const response = await fetch('http://localhost:8080/auth/client/register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    username: form.data.username,
                    phoneNumber: form.data.phoneNumber,
                    password: form.data.password
                })
            });

            if (!response.ok) {
                const error = await response.json();
                return fail(response.status, { form, error: error.message || 'Registration failed' });
            }

            const data = await response.json();

            // Set Cookie
            cookies.set('token', data.token, {
                path: '/',
                httpOnly: true,
                sameSite: 'strict',
                secure: false, // Dev only, true in prod
                maxAge: 60 * 60 // 1 hour
            });

        } catch (err) {
            return fail(500, { form, error: 'Network error or Gateway unreachable' });
        }

        throw redirect(303, '/');
    }
};
