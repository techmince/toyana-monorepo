using System.Security.Claims;

namespace Toyana.Shared;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                 ?? principal.FindFirst("sub")?.Value;
                 
        return Guid.TryParse(id, out var guid) ? guid : null;
    }

    public static Guid? GetVendorId(this ClaimsPrincipal principal)
    {
        var id = principal.FindFirst("vendorId")?.Value;
        return Guid.TryParse(id, out var guid) ? guid : null;
    }
    
    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value 
               ?? principal.FindFirst("email")?.Value;
    }
}
