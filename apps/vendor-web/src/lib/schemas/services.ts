import { z } from "zod";

export const serviceSchema = z.object({
    name: z.string().min(1, "Name is required"),
    description: z.string().min(10, "Description is too short"),
    price: z.number().min(0, "Price must be positive"),
    durationMinutes: z.number().min(15, "Duration must be at least 15 minutes"),
    category: z.string().min(1, "Category is required")
});

export type ServiceSchema = typeof serviceSchema;
