<script lang="ts">
	import { RadioGroup as RadioGroupPrimitive } from "bits-ui";
	import { cn } from "$lib/utils.js";

	let { ref = $bindable(null), value = $bindable(), class: className, children, ...restProps } = $props();
</script>

<RadioGroupPrimitive.Root
	bind:ref
	bind:value
	class={cn("grid gap-2", className)}
	{...restProps}
>
	{@render children?.()}
</RadioGroupPrimitive.Root>
