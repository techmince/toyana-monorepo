<script lang="ts">
	import * as Form from "$lib/components/ui/form";
	import { Input } from "$lib/components/ui/input";
    import { Button } from "$lib/components/ui/button";
    import * as Card from "$lib/components/ui/card";
	import { registerOwnerSchema, type RegisterOwnerSchema } from "$lib/schemas/auth";
	import { type SuperValidated, type Infer, superForm } from "sveltekit-superforms";
	import { zodClient } from "sveltekit-superforms/adapters";

	export let data: { form: SuperValidated<Infer<RegisterOwnerSchema>> };

	const form = superForm(data.form, {
		validators: zodClient(registerOwnerSchema)
	});

	const { form: formData, enhance } = form;
</script>

<div class="flex min-h-screen items-center justify-center bg-muted/50 py-8">
    <Card.Root class="w-full max-w-lg">
        <Card.Header>
            <Card.Title class="text-2xl font-bold text-center">Become a Partner</Card.Title>
            <Card.Description class="text-center">Register your business on Toyana</Card.Description>
        </Card.Header>
        <Card.Content>
            <form method="POST" use:enhance class="space-y-4">
                
                <div class="grid grid-cols-2 gap-4">
                     <Form.Field {form} name="username">
                        <Form.Control let:attrs>
                            <Form.Label>Username (Login)</Form.Label>
                            <Input {...attrs} bind:value={$formData.username} />
                        </Form.Control>
                        <Form.FieldErrors />
                    </Form.Field>

                     <Form.Field {form} name="businessName">
                        <Form.Control let:attrs>
                            <Form.Label>Business Name</Form.Label>
                            <Input {...attrs} bind:value={$formData.businessName} />
                        </Form.Control>
                        <Form.FieldErrors />
                    </Form.Field>
                </div>

                <div class="grid grid-cols-2 gap-4">
                    <Form.Field {form} name="category">
                       <Form.Control let:attrs>
                           <Form.Label>Category</Form.Label>
                           <Input {...attrs} placeholder="e.g. Catering" bind:value={$formData.category} />
                       </Form.Control>
                       <Form.FieldErrors />
                   </Form.Field>

                    <Form.Field {form} name="taxId">
                       <Form.Control let:attrs>
                           <Form.Label>Tax ID</Form.Label>
                           <Input {...attrs} bind:value={$formData.taxId} />
                       </Form.Control>
                       <Form.FieldErrors />
                   </Form.Field>
               </div>

                <Form.Field {form} name="password">
                    <Form.Control let:attrs>
                        <Form.Label>Password</Form.Label>
                        <Input {...attrs} type="password" bind:value={$formData.password} />
                    </Form.Control>
                    <Form.FieldErrors />
                </Form.Field>

                <Form.Field {form} name="confirmPassword">
                    <Form.Control let:attrs>
                        <Form.Label>Confirm Password</Form.Label>
                        <Input {...attrs} type="password" bind:value={$formData.confirmPassword} />
                    </Form.Control>
                    <Form.FieldErrors />
                </Form.Field>

                <Button type="submit" class="w-full">Register Business</Button>
            </form>
        </Card.Content>
        <Card.Footer class="justify-center">
            <p class="text-sm text-muted-foreground">
                Already registered? <a href="/login" class="underline text-primary">Login</a>
            </p>
        </Card.Footer>
    </Card.Root>
</div>
