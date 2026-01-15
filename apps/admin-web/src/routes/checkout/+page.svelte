<script lang="ts">
    import { Button } from "$lib/components/ui/button";
    import * as Card from "$lib/components/ui/card";
    import { Input } from "$lib/components/ui/input";
    import { Label } from "$lib/components/ui/label";
    import { Separator } from "$lib/components/ui/separator";
    import * as RadioGroup from "$lib/components/ui/radio-group";
    import { CreditCard, Wallet } from "lucide-svelte";
    import { enhance } from '$app/forms';

    let step = 1;
    
    // Mock Data (would come from URL params or store)
    const bookingDetails = {
        vendorName: "Creative Lens Studio",
        serviceName: "Full Wedding Package",
        date: "2024-06-15",
        price: 3500
    };

    function nextStep() { step++; }
    function prevStep() { step--; }
</script>

<div class="container max-w-2xl py-12 px-4">
    <div class="mb-8">
        <h1 class="text-3xl font-bold text-center">Checkout</h1>
        <div class="flex justify-center mt-4 gap-2">
            <div class={`h-2 w-16 rounded-full transition-colors ${step >= 1 ? 'bg-primary' : 'bg-muted'}`}></div>
            <div class={`h-2 w-16 rounded-full transition-colors ${step >= 2 ? 'bg-primary' : 'bg-muted'}`}></div>
            <div class={`h-2 w-16 rounded-full transition-colors ${step >= 3 ? 'bg-primary' : 'bg-muted'}`}></div>
        </div>
    </div>

    <form method="POST" use:enhance>
        <!-- Hidden Inputs for Submission -->
        <input type="hidden" name="vendorId" value="1" />
        <input type="hidden" name="serviceId" value="2" />
        <input type="hidden" name="date" value={bookingDetails.date} />

        {#if step === 1}
            <Card.Root>
                <Card.Header>
                    <Card.Title>1. Review Details</Card.Title>
                </Card.Header>
                <Card.Content class="space-y-4">
                    <div class="flex justify-between">
                        <span class="text-muted-foreground">Vendor</span>
                        <span class="font-medium">{bookingDetails.vendorName}</span>
                    </div>
                    <div class="flex justify-between">
                        <span class="text-muted-foreground">Service</span>
                        <span class="font-medium">{bookingDetails.serviceName}</span>
                    </div>
                    <div class="flex justify-between">
                        <span class="text-muted-foreground">Date</span>
                        <span class="font-medium">{bookingDetails.date}</span>
                    </div>
                    <Separator />
                    <div class="flex justify-between text-lg font-bold">
                        <span>Total</span>
                        <span>${bookingDetails.price}</span>
                    </div>
                </Card.Content>
                <Card.Footer class="flex justify-end">
                    <Button on:click={nextStep}>Next: Payment</Button>
                </Card.Footer>
            </Card.Root>
        {:else if step === 2}
             <Card.Root>
                <Card.Header>
                    <Card.Title>2. Payment Method</Card.Title>
                </Card.Header>
                <Card.Content class="space-y-4">
                    <RadioGroup.Root value="card" name="paymentMethod">
                        <div class="flex items-center space-x-2 border p-4 rounded-lg cursor-pointer hover:bg-muted/50">
                            <RadioGroup.Item value="card" id="card" />
                            <Label for="card" class="flex items-center gap-2 cursor-pointer flex-1">
                                <CreditCard class="h-5 w-5" />
                                <span>Credit Card ending in 4242</span>
                            </Label>
                        </div>
                        <div class="flex items-center space-x-2 border p-4 rounded-lg cursor-pointer hover:bg-muted/50">
                            <RadioGroup.Item value="wallet" id="wallet" />
                            <Label for="wallet" class="flex items-center gap-2 cursor-pointer flex-1">
                                <Wallet class="h-5 w-5" />
                                <span>Toyana Wallet ($500.00)</span>
                            </Label>
                        </div>
                    </RadioGroup.Root>
                </Card.Content>
                <Card.Footer class="flex justify-between">
                    <Button variant="outline" on:click={prevStep}>Back</Button>
                    <Button on:click={nextStep}>Next: Confirm</Button>
                </Card.Footer>
            </Card.Root>
        {:else if step === 3}
             <Card.Root>
                <Card.Header>
                    <Card.Title>3. Confirm Booking</Card.Title>
                    <Card.Description>Please review one last time before confirming.</Card.Description>
                </Card.Header>
                <Card.Content class="space-y-2">
                     <p>By clicking "Confirm Booking", you agree to the Terms of Service. The vendor will have 24 hours to accept your request.</p>
                </Card.Content>
                <Card.Footer class="flex justify-between">
                    <Button variant="outline" on:click={prevStep}>Back</Button>
                    <Button type="submit">Confirm Booking</Button>
                </Card.Footer>
            </Card.Root>
        {/if}
    </form>
</div>
