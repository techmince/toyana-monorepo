<script lang="ts">
  import { onMount } from 'svelte';
  
  let payments: any[] = [];
  
  onMount(async () => {
    const token = localStorage.getItem('token');
    if (!token) return;
    
    const res = await fetch('/api/admin/payments', {
        headers: { 'Authorization': `Bearer ${token}` }
    });
    if (res.ok) payments = await res.json();
  });
</script>

<div>
  <h1 class="text-2xl font-bold mb-4">Payments</h1>
  <div class="bg-white shadow overflow-hidden sm:rounded-lg">
    <table class="min-w-full divide-y divide-gray-200">
      <thead class="bg-gray-50">
        <tr>
          <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">ID</th>
          <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Amount</th>
          <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Net Amount</th>
          <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
          <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Fees</th>
        </tr>
      </thead>
      <tbody class="bg-white divide-y divide-gray-200">
        {#each payments as payment}
          <tr>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{payment.Id}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">${payment.TotalAmount}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-green-600">${payment.NetAmount}</td>
             <td class="px-6 py-4 whitespace-nowrap text-sm">
                <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full {payment.Status === 'Captured' ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'}">
                    {payment.Status}
                </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">${payment.PlatformFees}</td>
          </tr>
        {/each}
      </tbody>
    </table>
  </div>
</div>
