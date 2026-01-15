import { z } from "zod";

export const loginSchema = z.object({
    login: z.string().min(1, "Username or Phone is required"),
    password: z.string().min(1, "Password is required")
});

export const registerSchema = z.object({
    username: z.string().min(3, "Username must be at least 3 characters"),
    phoneNumber: z.string().min(5, "Phone number is required"), // Basic check
    password: z.string().min(6, "Password must be at least 6 characters"),
    confirmPassword: z.string().min(6, "Confirm Password is required")
}).refine((data) => data.password === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"]
});

export type LoginSchema = typeof loginSchema;
export type RegisterSchema = typeof registerSchema;
