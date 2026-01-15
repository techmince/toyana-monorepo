import { z } from "zod";

export const loginSchema = z.object({
    login: z.string().min(1, "Username is required"),
    password: z.string().min(1, "Password is required")
});

export const registerOwnerSchema = z.object({
    username: z.string().min(3, "Username must be at least 3 characters"),
    password: z.string().min(6, "Password must be at least 6 characters"),
    confirmPassword: z.string(),
    businessName: z.string().min(1, "Business Name is required"),
    taxId: z.string().min(1, "Tax ID is required"),
    category: z.string().min(1, "Category is required")
}).refine((data) => data.password === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"]
});

export type LoginSchema = typeof loginSchema;
export type RegisterOwnerSchema = typeof registerOwnerSchema;
