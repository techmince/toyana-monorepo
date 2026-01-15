<script lang="ts">
  import { goto } from '$app/navigation';
  
  let username = '';
  let password = '';
  let error = '';

  async function login() {
    try {
      const response = await fetch('/api/auth/admin/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ login: username, password })
      });
      
      if (response.ok) {
        const data = await response.json();
        localStorage.setItem('token', data.token);
        goto('/');
      } else {
        error = 'Invalid credentials';
      }
    } catch (e) {
      error = 'Login failed';
    }
  }
</script>

<div class="min-h-screen flex items-center justify-center bg-gray-100">
  <div class="bg-white p-8 rounded shadow-md w-96">
    <h1 class="text-2xl font-bold mb-4">Admin Login</h1>
    {#if error}
      <div class="bg-red-100 text-red-700 p-2 mb-4 rounded">{error}</div>
    {/if}
    <input bind:value={username} class="w-full border p-2 mb-4 rounded" placeholder="Username" />
    <input type="password" bind:value={password} class="w-full border p-2 mb-4 rounded" placeholder="Password" />
    <button on:click={login} class="w-full bg-blue-600 text-white p-2 rounded hover:bg-blue-700">Login</button>
  </div>
</div>
