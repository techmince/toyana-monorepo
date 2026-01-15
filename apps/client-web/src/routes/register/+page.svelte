<script lang="ts">
	import * as Form from "$lib/components/ui/form";
	import { Input } from "$lib/components/ui/input";
    import { Button } from "$lib/components/ui/button";
    import * as Card from "$lib/components/ui/card";
	import { registerSchema, type RegisterSchema } from "$lib/schemas/auth";
	import { type SuperValidated, type Infer, superForm } from "sveltekit-superforms";
	import { zodClient } from "sveltekit-superforms/adapters";

	export let data: { form: SuperValidated<Infer<RegisterSchema>> };

	const form = superForm(data.form, {
		validators: zodClient(registerSchema)
	});

	const { form: formData, enhance } = form;
</script>

<div class="flex min-h-screen items-center justify-center bg-muted/50">
    <Card.Root class="w-full max-w-md">
        <Card.Header>
            <Card.Title class="text-2xl font-bold text-center">Create Account</Card.Title>
            <Card.Description class="text-center">Join Toyana Marketplace</Card.Description>
        </Card.Header>
        <Card.Content>
            <form method="POST" use:enhance class="space-y-4">
                <Form.Field {form} name="username">
                    <Form.Control let:attrs>
                        <Form.Label>Username</Form.Label>
                        <Input {...attrs} bind:value={$formData.username} />
                    </Form.Control>
                    <Form.FieldErrors />
                </Form.Field>

                <Form.Field {form} name="phoneNumber">
                    <Form.Control let:attrs>
                        <Form.Label>Phone Number</Form.Label>
                        <Input {...attrs} type="tel" bind:value={$formData.phoneNumber} />
                    </Form.Control>
                    <Form.FieldErrors />
                </Form.Field>

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

                <Button type="submit" class="w-full">Sign Up</Button>
            </form>
        </Card.Content>
        <Card.Footer class="justify-center">
            <p class="text-sm text-muted-foreground">
                Already have an account? <a href="/login" class="underline text-primary">Login</a>
            </p>
        </Card.Footer>
    </Card.Root>
</div>
