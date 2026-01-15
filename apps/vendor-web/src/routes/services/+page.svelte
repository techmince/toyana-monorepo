<script lang="ts">
    import { Button } from "$lib/components/ui/button";
    import * as Table from "$lib/components/ui/table";
    import { Plus } from "lucide-svelte";
    
    export let data;
</script>

<div class="p-8 space-y-6">
    <div class="flex justify-between items-center">
        <div>
            <h1 class="text-3xl font-bold">Services</h1>
            <p class="text-muted-foreground">Manage your offerings</p>
        </div>
        <Button href="/services/new">
            <Plus class="mr-2 h-4 w-4" /> Add Service
        </Button>
    </div>

    <div class="rounded-md border">
        <Table.Root>
            <Table.Header>
                <Table.Row>
                    <Table.Head>Name</Table.Head>
                    <Table.Head>Category</Table.Head>
                    <Table.Head>Price</Table.Head>
                    <Table.Head>Duration</Table.Head>
                    <Table.Head class="text-right">Actions</Table.Head>
                </Table.Row>
            </Table.Header>
            <Table.Body>
                {#if data.services.length === 0}
                    <Table.Row>
                        <Table.Cell colspan={5} class="text-center h-24 text-muted-foreground">
                            No services found. Add one to get started.
                        </Table.Cell>
                    </Table.Row>
                {:else}
                    {#each data.services as service}
                        <Table.Row>
                            <Table.Cell class="font-medium">{service.name}</Table.Cell>
                            <Table.Cell>{service.category}</Table.Cell>
                            <Table.Cell>${service.price.toFixed(2)}</Table.Cell>
                            <Table.Cell>{service.durationMinutes} mins</Table.Cell>
                            <Table.Cell class="text-right">
                                <Button variant="ghost" size="sm">Edit</Button>
                            </Table.Cell>
                        </Table.Row>
                    {/each}
                {/if}
            </Table.Body>
        </Table.Root>
    </div>
</div>
