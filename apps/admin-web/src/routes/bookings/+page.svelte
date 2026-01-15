<script lang="ts">
  import { onMount } from 'svelte';
  
  let bookings: any[] = [];
  
  onMount(async () => {
    const token = localStorage.getItem('token');
    if (!token) return;
    
    const res = await fetch('/api/admin/bookings', {
        headers: { 'Authorization': `Bearer ${token}` }
    });
    if (res.ok) bookings = await res.json();
  });
</script>

<div>
  <h1 class="text-2xl font-bold mb-4">Bookings</h1>
  <div class="bg-white shadow overflow-hidden sm:rounded-lg">
    <table class="min-w-full divide-y divide-gray-200">
      <thead class="bg-gray-50">
        <tr>
          <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">ID</th>
          <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date</th>
          <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
          <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
        </tr>
      </thead>
      <tbody class="bg-white divide-y divide-gray-200">
        {#each bookings as booking}
          <tr>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{booking.id}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{new Date(booking.eventDate).toLocaleDateString()}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{booking.status}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2">
              <button class="text-red-600 hover:text-red-900">Cancel</button>
              <a href="/bookings/{booking.id}/trace" class="text-blue-600 hover:text-blue-900">Trace</a>
            </td>
          </tr>
        {/each}
      </tbody>
    </table>
  </div>
</div>
