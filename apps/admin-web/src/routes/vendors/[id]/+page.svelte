<script lang="ts">
    import { Button } from "$lib/components/ui/button";
    import * as Card from "$lib/components/ui/card";
    import { Badge } from "$lib/components/ui/badge";
    import { Separator } from "$lib/components/ui/separator";
    import * as Tabs from "$lib/components/ui/tabs";
    import { Star, MapPin, Clock } from "lucide-svelte";
    
    export let data;
</script>

<div class="container py-8 px-4">
    <!-- Header -->
    <div class="flex flex-col md:flex-row justify-between items-start gap-4 mb-8">
        <div>
            <h1 class="text-4xl font-bold mb-2">{data.vendor.name}</h1>
            <div class="flex items-center gap-4 text-muted-foreground">
                <span class="flex items-center gap-1"><MapPin class="h-4 w-4" /> {data.vendor.location}</span>
                <span class="flex items-center gap-1 text-black font-semibold"><Star class="h-4 w-4 fill-yellow-400 text-yellow-400" /> {data.vendor.rating} ({data.vendor.reviews} reviews)</span>
            </div>
        </div>
        <div class="flex items-center gap-2">
            <Button variant="outline">Message</Button>
            <Button>Book Now</Button>
        </div>
    </div>

    <!-- Gallery Placeholder -->
    <div class="grid grid-cols-4 gap-2 mb-8 h-96 rounded-lg overflow-hidden">
        <div class="col-span-2 row-span-2 bg-muted flex items-center justify-center text-4xl">1</div>
        <div class="bg-muted flex items-center justify-center text-2xl">2</div>
        <div class="bg-muted flex items-center justify-center text-2xl">3</div>
        <div class="bg-muted flex items-center justify-center text-2xl">4</div>
        <div class="bg-muted flex items-center justify-center text-2xl">5</div>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <!-- Main Content -->
        <div class="md:col-span-2 space-y-8">
            <Tabs.Root value="about">
                <Tabs.List>
                    <Tabs.Trigger value="about">About</Tabs.Trigger>
                    <Tabs.Trigger value="services">Services</Tabs.Trigger>
                    <Tabs.Trigger value="reviews">Reviews</Tabs.Trigger>
                </Tabs.List>
                <Tabs.Content value="about" class="space-y-4 mt-4">
                    <p class="leading-relaxed text-muted-foreground">
                        {data.vendor.description}
                    </p>
                </Tabs.Content>
                <Tabs.Content value="services" class="space-y-4 mt-4">
                    {#each data.vendor.services as service}
                        <Card.Root>
                            <Card.Header>
                                <div class="flex justify-between items-start">
                                    <div>
                                        <Card.Title>{service.name}</Card.Title>
                                        <Card.Description>{service.duration} mins</Card.Description>
                                    </div>
                                    <div class="font-bold text-lg">${service.price}</div>
                                </div>
                            </Card.Header>
                            <Card.Content>
                                <p class="text-sm text-muted-foreground mb-4">{service.description}</p>
                                <Button size="sm" class="w-full">Select Package</Button>
                            </Card.Content>
                        </Card.Root>
                    {/each}
                </Tabs.Content>
            </Tabs.Root>
        </div>

        <!-- Sticky Sidebar -->
        <div class="space-y-6">
            <Card.Root>
                <Card.Header>
                    <Card.Title>Availability</Card.Title>
                </Card.Header>
                <Card.Content>
                    <div class="bg-muted h-64 rounded flex items-center justify-center text-muted-foreground">
                        Calendar Placeholder
                    </div>
                </Card.Content>
            </Card.Root>
        </div>
    </div>
</div>
