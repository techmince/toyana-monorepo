import { fail, redirect } from '@sveltejs/kit';

export const actions = {
    default: async ({ request, fetch, locals }) => {
        if (!locals.user?.token) return fail(401);

        const data = await request.formData();
        const vendorId = data.get('vendorId');
        const serviceId = data.get('serviceId');
        const date = data.get('date');
        const paymentMethod = data.get('paymentMethod');

        if (!vendorId || !serviceId || !date) {
            return fail(400, { error: 'Missing required fields' });
        }

        try {
            const res = await fetch('http://localhost:8080/ordering/bookings', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${locals.user.token}`
                },
                body: JSON.stringify({
                    vendorId,
                    serviceId,
                    date,
                    paymentMethodId: paymentMethod
                })
            });

            if (!res.ok) {
                return fail(res.status, { error: 'Booking failed' });
            }

        } catch (e) {
            return fail(500, { error: 'Network error ' });
        }

        throw redirect(303, '/profile'); // Redirect to My Bookings
    }
};
