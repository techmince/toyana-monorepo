<script lang="ts">
  import { onMount } from 'svelte';
  
  let stats = { clients: 0, vendors: 0, services: 0, bookings: 0 };
  
  onMount(async () => {
    const token = localStorage.getItem('token');
    if (!token) return; // Middleware should handle redirect
    
    const res = await fetch('/api/admin/dashboard', {
        headers: { 'Authorization': `Bearer ${token}` }
    });
    if (res.ok) stats = await res.json();
  });
</script>

<div class="p-8">
  <h1 class="text-3xl font-bold mb-6">Dashboard</h1>
  
  <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
    <div class="bg-white p-6 rounded-lg shadow">
      <h3 class="text-gray-500 text-sm font-medium">Total Clients</h3>
      <p class="text-3xl font-bold text-gray-900">{stats.clients}</p>
    </div>
    
    <div class="bg-white p-6 rounded-lg shadow">
      <h3 class="text-gray-500 text-sm font-medium">Total Vendors</h3>
      <p class="text-3xl font-bold text-gray-900">{stats.vendors}</p>
    </div>
    
    <div class="bg-white p-6 rounded-lg shadow">
      <h3 class="text-gray-500 text-sm font-medium">Active Services</h3>
      <p class="text-3xl font-bold text-gray-900">{stats.services}</p>
    </div>
    
    <div class="bg-white p-6 rounded-lg shadow">
      <h3 class="text-gray-500 text-sm font-medium">Total Bookings</h3>
      <p class="text-3xl font-bold text-gray-900">{stats.bookings}</p>
    </div>
  </div>

  <div class="mt-8">
      <h2 class="text-xl font-bold mb-4">Maintenance</h2>
      <button 
        class="bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700"
        on:click={async () => {
            if(!confirm('Flush Cache?')) return;
            const token = localStorage.getItem('token');
            await fetch('/api/admin/cache/flush', { method: 'POST', headers: { 'Authorization': `Bearer ${token}` } });
            alert('Cache Flushed');
        }}
      >
        Flush Valkey Cache
      </button>
  </div>
</div>
