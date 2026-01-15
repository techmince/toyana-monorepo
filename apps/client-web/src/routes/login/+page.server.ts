import { fail, redirect } from '@sveltejs/kit';
import { superValidate } from 'sveltekit-superforms';
import { zod } from 'sveltekit-superforms/adapters';
import { loginSchema } from '$lib/schemas/auth';

export const load = async () => {
    const form = await superValidate(zod(loginSchema));
    return { form };
};

export const actions = {
    default: async ({ request, fetch, cookies }) => {
        const form = await superValidate(request, zod(loginSchema));

        if (!form.valid) {
            return fail(400, { form });
        }

        try {
            const response = await fetch('http://localhost:8080/auth/client/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    login: form.data.login,
                    password: form.data.password
                })
            });

            if (!response.ok) {
                // Gateway returns 401 on failure
                return fail(401, { form, error: 'Invalid credentials' });
            }

            const data = await response.json();

            cookies.set('token', data.token, {
                path: '/',
                httpOnly: true,
                sameSite: 'strict',
                secure: false,
                maxAge: 60 * 60
            });

        } catch (err) {
            return fail(500, { form, error: 'Network error' });
        }

        throw redirect(303, '/');
    }
};
