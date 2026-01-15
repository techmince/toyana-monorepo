<script lang="ts">
	import * as Form from "$lib/components/ui/form";
	import { Input } from "$lib/components/ui/input";
    import { Button } from "$lib/components/ui/button";
    import * as Card from "$lib/components/ui/card";
	import { loginSchema, type LoginSchema } from "$lib/schemas/auth";
	import { type SuperValidated, type Infer, superForm } from "sveltekit-superforms";
	import { zodClient } from "sveltekit-superforms/adapters";

	export let data: { form: SuperValidated<Infer<LoginSchema>> };

	const form = superForm(data.form, {
		validators: zodClient(loginSchema)
	});

	const { form: formData, enhance } = form;
</script>

<div class="flex min-h-screen items-center justify-center bg-muted/50">
    <Card.Root class="w-full max-w-md">
        <Card.Header>
            <Card.Title class="text-2xl font-bold text-center">Vendor Portal</Card.Title>
            <Card.Description class="text-center">Manage your business</Card.Description>
        </Card.Header>
        <Card.Content>
            <form method="POST" use:enhance class="space-y-4">
                <Form.Field {form} name="login">
                    <Form.Control let:attrs>
                        <Form.Label>Username</Form.Label>
                        <Input {...attrs} bind:value={$formData.login} />
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

                <Button type="submit" class="w-full">Login</Button>
            </form>
        </Card.Content>
        <Card.Footer class="justify-center">
            <p class="text-sm text-muted-foreground">
                New Vendor? <a href="/register" class="underline text-primary">Partner with us</a>
            </p>
        </Card.Footer>
    </Card.Root>
</div>
