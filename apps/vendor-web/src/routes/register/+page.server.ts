import { fail, redirect } from '@sveltejs/kit';
import { superValidate } from 'sveltekit-superforms';
import { zod } from 'sveltekit-superforms/adapters';
import { registerOwnerSchema } from '$lib/schemas/auth';

export const load = async () => {
    const form = await superValidate(zod(registerOwnerSchema));
    return { form };
};

export const actions = {
    default: async ({ request, fetch, cookies }) => {
        const form = await superValidate(request, zod(registerOwnerSchema));

        if (!form.valid) {
            return fail(400, { form });
        }

        try {
            const response = await fetch('http://localhost:8080/auth/vendor/register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    username: form.data.username,
                    password: form.data.password,
                    businessName: form.data.businessName,
                    taxId: form.data.taxId,
                    category: form.data.category
                })
            });

            if (!response.ok) {
                const error = await response.json();
                return fail(response.status, { form, error: error.message || 'Registration failed' });
            }

            const data = await response.json();

            cookies.set('token', data.token, {
                path: '/',
                httpOnly: true,
                sameSite: 'strict',
                secure: false, // Dev
                maxAge: 60 * 60
            });

        } catch (err) {
            return fail(500, { form, error: 'Network error' });
        }

        throw redirect(303, '/dashboard');
    }
};
