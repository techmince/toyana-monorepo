<script lang="ts">
    import * as Form from "$lib/components/ui/form";
    import { Input } from "$lib/components/ui/input";
    import * as Card from "$lib/components/ui/card";
    import { Button } from "$lib/components/ui/button";
    import { Textarea } from "$lib/components/ui/textarea"; // Assuming added
    import { serviceSchema, type ServiceSchema } from "$lib/schemas/services";
    import { type SuperValidated, type Infer, superForm } from "sveltekit-superforms";
    import { zodClient } from "sveltekit-superforms/adapters";

    export let data: { form: SuperValidated<Infer<ServiceSchema>> };

    const form = superForm(data.form, {
        validators: zodClient(serviceSchema)
    });

    const { form: formData, enhance } = form;
</script>

<div class="flex min-h-screen items-center justify-center bg-muted/50 p-8">
    <Card.Root class="w-full max-w-lg">
        <Card.Header>
            <Card.Title>Add New Service</Card.Title>
            <Card.Description>Define a service you offer.</Card.Description>
        </Card.Header>
        <Card.Content>
            <form method="POST" use:enhance class="space-y-4">
                <Form.Field {form} name="name">
                    <Form.Control let:attrs>
                        <Form.Label>Service Name</Form.Label>
                        <Input {...attrs} bind:value={$formData.name} placeholder="e.g. Wedding Photography" />
                    </Form.Control>
                    <Form.FieldErrors />
                </Form.Field>

                <Form.Field {form} name="category">
                    <Form.Control let:attrs>
                        <Form.Label>Category</Form.Label>
                        <Input {...attrs} bind:value={$formData.category} placeholder="e.g. Photography" />
                    </Form.Control>
                    <Form.FieldErrors />
                </Form.Field>

                <Form.Field {form} name="description">
                    <Form.Control let:attrs>
                         <Form.Label>Description</Form.Label>
                         <Textarea {...attrs} bind:value={$formData.description} placeholder="Describe the service..." />
                    </Form.Control>
                    <Form.FieldErrors />
                </Form.Field>

                <div class="grid grid-cols-2 gap-4">
                    <Form.Field {form} name="price">
                        <Form.Control let:attrs>
                            <Form.Label>Price ($)</Form.Label>
                            <Input {...attrs} type="number" step="0.01" bind:value={$formData.price} />
                        </Form.Control>
                        <Form.FieldErrors />
                    </Form.Field>

                    <Form.Field {form} name="durationMinutes">
                        <Form.Control let:attrs>
                            <Form.Label>Duration (mins)</Form.Label>
                            <Input {...attrs} type="number" step="15" bind:value={$formData.durationMinutes} />
                        </Form.Control>
                        <Form.FieldErrors />
                    </Form.Field>
                </div>

                <div class="flex justify-end gap-2">
                    <Button variant="outline" href="/services">Cancel</Button>
                    <Button type="submit">Create Service</Button>
                </div>
            </form>
        </Card.Content>
    </Card.Root>
</div>
