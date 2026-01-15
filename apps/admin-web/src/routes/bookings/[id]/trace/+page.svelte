<script lang="ts">
  import { onMount } from 'svelte';
  import { page } from '$app/stores';

  let events: any[] = [];
  let bookingId = $page.params.id;

  onMount(async () => {
    const token = localStorage.getItem('token');
    const res = await fetch(`/api/admin/bookings/${bookingId}/trace`, {
        headers: { 'Authorization': `Bearer ${token}` }
    });
    if (res.ok) events = await res.json();
  });
</script>

<div class="p-6">
  <h1 class="text-2xl font-bold mb-6">Booking Trace: {bookingId}</h1>
  
  <div class="flow-root">
    <ul role="list" class="-mb-8">
      {#each events as event, i}
        <li>
          <div class="relative pb-8">
            {#if i !== events.length - 1}
              <span class="absolute top-4 left-4 -ml-px h-full w-0.5 bg-gray-200" aria-hidden="true"></span>
            {/if}
            <div class="relative flex space-x-3">
              <div>
                <span class="h-8 w-8 rounded-full bg-blue-500 flex items-center justify-center ring-8 ring-white">
                    <!-- Icon placeholder -->
                </span>
              </div>
              <div class="min-w-0 flex-1 pt-1.5 flex justify-between space-x-4">
                <div>
                  <p class="text-sm text-gray-500">{event.type} <span class="font-medium text-gray-900">occurred</span></p>
                   <pre class="mt-2 text-xs bg-gray-100 p-2 rounded">{JSON.stringify(event.data, null, 2)}</pre>
                </div>
                <div class="text-right text-sm whitespace-nowrap text-gray-500">
                  <time>{new Date(event.timestamp).toLocaleString()}</time>
                </div>
              </div>
            </div>
          </div>
        </li>
      {/each}
    </ul>
  </div>
</div>
