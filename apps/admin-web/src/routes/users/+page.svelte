<script lang="ts">
  import { onMount } from 'svelte';
  
  let users: any[] = [];
  
  onMount(async () => {
    const token = localStorage.getItem('token');
    if (!token) return;
    
    const res = await fetch('/api/admin/users', {
        headers: { 'Authorization': `Bearer ${token}` }
    });
    if (res.ok) users = await res.json();
  });
</script>

<div>
  <h1 class="text-2xl font-bold mb-4">Users</h1>
  <div class="bg-white shadow overflow-hidden sm:rounded-lg">
    <table class="min-w-full divide-y divide-gray-200">
      <thead class="bg-gray-50">
        <tr>
          <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Username</th>
          <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Type</th>
          <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
        </tr>
      </thead>
      <tbody class="bg-white divide-y divide-gray-200">
        {#each users as user}
          <tr>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{user.username}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{user.type}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2">
              <button 
                  class="text-red-600 hover:text-red-900"
                  on:click={async () => {
                      if(!confirm(`Ban ${user.username}?`)) return;
                      const token = localStorage.getItem('token');
                      await fetch(`/api/admin/users/${user.id}/ban`, { method: 'POST', headers: { 'Authorization': `Bearer ${token}` } });
                      alert('User Banned');
                  }}
              >Ban</button>
              
              <button 
                  class="text-yellow-600 hover:text-yellow-900"
                  on:click={async () => {
                      const newPass = prompt(`New Password for ${user.username}:`);
                      if(!newPass) return;
                      const token = localStorage.getItem('token');
                      await fetch(`/api/admin/users/${user.id}/password`, { 
                          method: 'POST', 
                          headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
                          body: JSON.stringify(newPass)
                      });
                      alert('Password Reset');
                  }}
              >Reset Password</button>
            </td>
          </tr>
        {/each}
      </tbody>
    </table>
  </div>
</div>
