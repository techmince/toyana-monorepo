<script lang="ts">
    import { Button } from "$lib/components/ui/button";
    import * as Card from "$lib/components/ui/card";
    import { Input } from "$lib/components/ui/input";
    import { Search, MapPin, Camera, Music, Utensils, Flower2 } from "lucide-svelte";
    import { goto } from '$app/navigation';

    let searchQuery = "";

    function handleSearch() {
        if (searchQuery) {
            goto(`/search?q=${searchQuery}`);
        }
    }

    const categories = [
        { name: "Photographers", icon: Camera, color: "text-blue-500", bg: "bg-blue-100" },
        { name: "Catering", icon: Utensils, color: "text-orange-500", bg: "bg-orange-100" },
        { name: "Florists", icon: Flower2, color: "text-pink-500", bg: "bg-pink-100" },
        { name: "Music & DJs", icon: Music, color: "text-purple-500", bg: "bg-purple-100" },
    ];
</script>

<div class="flex flex-col min-h-screen">
    <!-- Hero Section -->
    <section class="relative h-[500px] flex items-center justify-center bg-gradient-to-r from-primary/10 to-primary/5">
        <div class="container px-4 text-center space-y-6">
            <h1 class="text-5xl font-extrabold tracking-tight sm:text-6xl">
                Plan Your Perfect Event <br/> with <span class="text-primary">Toyana</span>
            </h1>
            <p class="text-xl text-muted-foreground max-w-2xl mx-auto">
                Discover and book top-rated vendors for weddings, parties, and corporate events. Trusted by thousands.
            </p>

            <!-- Main Search Bar -->
            <div class="max-w-xl mx-auto flex gap-2 p-2 bg-background rounded-full shadow-lg border">
                <div class="flex-1 flex items-center px-4 gap-2">
                    <Search class="h-5 w-5 text-muted-foreground" />
                    <input 
                        bind:value={searchQuery}
                        on:keydown={(e) => e.key === 'Enter' && handleSearch()}
                        type="text" 
                        placeholder="What are you looking for? (e.g. Wedding Photographer)" 
                        class="flex-1 bg-transparent outline-none border-none placeholder:text-muted-foreground/50"
                    />
                </div>
                <div class="h-8 w-[1px] bg-border my-auto"></div>
                <div class="flex-1 flex items-center px-4 gap-2">
                    <MapPin class="h-5 w-5 text-muted-foreground" />
                    <input type="text" placeholder="Location" class="flex-1 bg-transparent outline-none border-none placeholder:text-muted-foreground/50" />
                </div>
                <Button size="lg" class="rounded-full px-8" on:click={handleSearch}>Search</Button>
            </div>
        </div>
    </section>

    <!-- Categories Section -->
    <section class="py-16 bg-muted/30">
        <div class="container px-4">
             <h2 class="text-3xl font-bold text-center mb-10">Browse by Category</h2>
             <div class="grid grid-cols-2 md:grid-cols-4 gap-6">
                {#each categories as cat}
                    <a href={`/search?category=${cat.name}`} class="group">
                        <Card.Root class="hover:shadow-lg transition-all cursor-pointer h-full border-none shadow-sm">
                            <Card.Content class="flex flex-col items-center justify-center p-8 gap-4">
                                <div class={`p-4 rounded-full ${cat.bg}`}>
                                    <svelte:component this={cat.icon} class={`h-8 w-8 ${cat.color}`} />
                                </div>
                                <h3 class="font-semibold text-lg group-hover:text-primary transition-colors">{cat.name}</h3>
                            </Card.Content>
                        </Card.Root>
                    </a>
                {/each}
             </div>
        </div>
    </section>

    <!-- Featured Vendors Section (Placeholder for now) -->
    <section class="py-16">
        <div class="container px-4">
            <h2 class="text-3xl font-bold text-center mb-4">Trending Vendors</h2>
            <p class="text-center text-muted-foreground mb-10">Top picks from our community this week.</p>
            
            <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                {#each [1, 2, 3] as i}
                    <Card.Root>
                        <Card.Header class="p-0">
                            <div class="aspect-video bg-muted w-full relative">
                                <span class="absolute top-2 right-2 bg-white/90 px-2 py-1 rounded text-xs font-bold">4.9 ★</span>
                            </div>
                        </Card.Header>
                        <Card.Content class="p-6">
                            <h3 class="font-bold text-xl mb-1">Creative Lens Studio</h3>
                            <p class="text-sm text-muted-foreground mb-4">Photography • New York, NY</p>
                            <div class="flex justify-between items-center">
                                <span class="font-semibold text-primary">From $1,200</span>
                                <Button variant="outline" size="sm" href={`/vendors/${i}`}>View Profile</Button>
                            </div>
                        </Card.Content>
                    </Card.Root>
                {/each}
            </div>
        </div>
    </section>
</div>
