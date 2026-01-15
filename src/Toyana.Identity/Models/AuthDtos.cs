namespace Toyana.Identity.Models;

public record RegisterRequest(string Username, string PhoneNumber, string Password);
public record VendorRegisterRequest(string Username, string Password, string BusinessName, string TaxId, string Category); // Creates Vendor + Owner User
public record LoginRequest(string Login, string Password);
public record AuthResponse(string Token, string RefreshToken, DateTime Expiration);

public record CreateSubUserRequest(string Username, string Password, List<string> Roles, List<string> ExtraPermissions);
