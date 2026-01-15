<script lang="ts">
    import * as Card from "$lib/components/ui/card";
    import { Button } from "$lib/components/ui/button";
</script>

<div class="p-8 space-y-8">
    <h1 class="text-3xl font-bold">Vendor Dashboard</h1>
    
    <div class="grid gap-4 md:grid-cols-3">
        <Card.Root>
            <Card.Header>
                <Card.Title>Total Revenue</Card.Title>
            </Card.Header>
            <Card.Content>
                <div class="text-2xl font-bold">$0.00</div>
            </Card.Content>
        </Card.Root>
        
        <Card.Root>
            <Card.Header>
                <Card.Title>Upcoming Bookings</Card.Title>
            </Card.Header>
            <Card.Content>
                 <div class="text-2xl font-bold">0</div>
            </Card.Content>
        </Card.Root>

        <Card.Root>
            <Card.Header>
                <Card.Title>Services Active</Card.Title>
            </Card.Header>
            <Card.Content>
                 <div class="text-2xl font-bold">--</div>
            </Card.Content>
        </Card.Root>
    </div>

    <div>
        <Button variant="outline">Manage Services</Button>
        <Button variant="outline">View Calendar</Button>
    </div>
</div>
