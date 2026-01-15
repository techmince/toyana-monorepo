<script lang="ts">
	import { dndzone } from 'svelte-dnd-action';
	import { flip } from 'svelte/animate';
    import * as Card from "$lib/components/ui/card";
    import { Badge } from "$lib/components/ui/badge";

	const flipDurationMs = 200;

    type Order = {
        id: number; 
        title: string; 
        price: string;
        customer: string;
    };

	let columns = [
		{
			id: 'pending',
			name: 'Pending',
			items: [
				{ id: 1, title: 'Wedding Photos', price: '$1,200', customer: 'Alice Smith' },
				{ id: 2, title: 'Engagement Shoot', price: '$400', customer: 'Bob Jones' }
			] as Order[]
		},
		{
			id: 'approved',
			name: 'Approved',
			items: [
                { id: 3, title: 'Corporate Event', price: '$2,500', customer: 'Tech Corp' }
            ] as Order[]
		},
		{
			id: 'completed',
			name: 'Completed',
			items: [] as Order[]
		}
	];

	function handleDndConsider(cid: string, e: CustomEvent<DndEvent<Order>>) {
		const colIdx = columns.findIndex(c => c.id === cid);
		columns[colIdx].items = e.detail.items;
		columns = [...columns];
	}

	function handleDndFinalize(cid: string, e: CustomEvent<DndEvent<Order>>) {
		const colIdx = columns.findIndex(c => c.id === cid);
		columns[colIdx].items = e.detail.items;
		columns = [...columns];
        // TODO: Call API to update status
	}
</script>

<div class="p-8 space-y-6">
    <h1 class="text-3xl font-bold">Orders Board</h1>
    
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 h-[calc(100vh-200px)]">
        {#each columns as column (column.id)}
            <div class="flex flex-col h-full rounded-lg bg-muted/50 border p-4">
                <h2 class="font-semibold mb-4 flex justify-between items-center">
                    {column.name}
                    <Badge variant="secondary">{column.items.length}</Badge>
                </h2>
                
                <div 
                    class="flex-1 space-y-3 min-h-[100px]"
                    use:dndzone={{items: column.items, flipDurationMs}} 
                    on:consider={(e) => handleDndConsider(column.id, e)} 
                    on:finalize={(e) => handleDndFinalize(column.id, e)}
                >
                    {#each column.items as item (item.id)}
                        <div animate:flip={{duration: flipDurationMs}}>
                             <Card.Root class="cursor-move hover:shadow-md transition-shadow">
                                <Card.Header class="p-4 pb-2">
                                    <Card.Title class="text-sm font-medium">{item.title}</Card.Title>
                                    <Card.Description class="text-xs">{item.customer}</Card.Description>
                                </Card.Header>
                                <Card.Content class="p-4 pt-0">
                                    <div class="font-bold text-sm mt-2">{item.price}</div>
                                </Card.Content>
                            </Card.Root>
                        </div>
                    {/each}
                </div>
            </div>
        {/each}
    </div>
</div>
