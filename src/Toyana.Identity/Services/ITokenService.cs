using Toyana.Identity.Models;

namespace Toyana.Identity.Services;

public interface ITokenService
{
    Task<AuthResponse> GenerateTokenAsync(string userId, string role, Guid? vendorId = null, bool isOwner = false, List<string> permissions = null);
}
