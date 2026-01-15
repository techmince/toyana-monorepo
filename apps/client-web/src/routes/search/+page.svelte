<script lang="ts">
    import * as Card from "$lib/components/ui/card";
    import { Button } from "$lib/components/ui/button";
    import { Checkbox } from "$lib/components/ui/checkbox";
    import { Label } from "$lib/components/ui/label";
    import { Input } from "$lib/components/ui/input";
    import { Separator } from "$lib/components/ui/separator";

    export let data; // Loaded from +page.server.ts
</script>

<div class="container py-8 px-4 flex flex-col md:flex-row gap-8">
    <!-- Filters Sidebar -->
    <aside class="w-full md:w-64 space-y-6 flex-shrink-0">
        <div>
            <h3 class="font-semibold mb-4">Categories</h3>
            <div class="space-y-2">
                {#each ['Photography', 'Catering', 'Music', 'Venues'] as cat}
                    <div class="flex items-center space-x-2">
                        <Checkbox id={cat} />
                        <Label for={cat}>{cat}</Label>
                    </div>
                {/each}
            </div>
        </div>
        <Separator />
        <div>
            <h3 class="font-semibold mb-4">Price Range</h3>
            <div class="flex items-center gap-2">
                <Input type="number" placeholder="Min" class="w-20" />
                <span>-</span>
                <Input type="number" placeholder="Max" class="w-20" />
            </div>
        </div>
        <Separator />
        <Button class="w-full">Apply Filters</Button>
    </aside>

    <!-- Results Grid -->
    <main class="flex-1">
        <div class="flex justify-between items-center mb-6">
            <h1 class="text-2xl font-bold">Search Results</h1>
            <p class="text-muted-foreground">{data.results.length} found</p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {#each data.results as vendor}
                <Card.Root class="hover:shadow-md transition-shadow cursor-pointer">
                    <div class="aspect-[4/3] bg-muted relative rounded-t-lg overflow-hidden">
                        <!-- Placeholder Image -->
                        <div class="absolute inset-0 flex items-center justify-center text-muted-foreground/30 text-4xl">📷</div>
                    </div>
                    <Card.Content class="p-4">
                        <div class="flex justify-between items-start mb-2">
                            <div>
                                <h3 class="font-bold text-lg">{vendor.name}</h3>
                                <p class="text-sm text-muted-foreground">{vendor.category}</p>
                            </div>
                            <div class="flex items-center bg-green-100 text-green-800 px-2 py-0.5 rounded text-xs font-bold">
                                ★ {vendor.rating}
                            </div>
                        </div>
                        <p class="text-sm text-muted-foreground line-clamp-2 mb-4">
                            {vendor.description}
                        </p>
                        <div class="flex items-center justify-between mt-auto">
                            <span class="font-bold">${vendor.basePrice}</span>
                            <Button size="sm" href={`/vendors/${vendor.id}`}>View Details</Button>
                        </div>
                    </Card.Content>
                </Card.Root>
            {/each}
        </div>
    </main>
</div>
