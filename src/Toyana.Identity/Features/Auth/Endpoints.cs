using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Wolverine.Http;
using Toyana.Identity.Services;
using Toyana.Identity.Models;
using System.Security.Claims;

namespace Toyana.Identity.Features.Auth;

// Grouping all Auth endpoints here
public static class AuthEndpoints
{
    [WolverinePost("/auth/client/register")]
    [Tags("Auth")]
    public static async Task<IResult> RegisterClient(
        RegisterRequest request, 
        AuthService auth)
    {
        try 
        { 
            return Results.Ok(await auth.RegisterClientAsync(request)); 
        }
        catch (Exception ex) 
        { 
            return Results.BadRequest(ex.Message); 
        }
    }

    [WolverinePost("/auth/client/login")]
    [Tags("Auth")]
    public static async Task<IResult> LoginClient(
        LoginRequest request, 
        AuthService auth)
    {
        try 
        { 
            return Results.Ok(await auth.LoginClientAsync(request)); 
        }
        catch (Exception ex) 
        { 
            return Results.Unauthorized(); 
        }
    }

    [WolverinePost("/auth/vendor/register")]
    [Tags("Auth")]
    public static async Task<IResult> RegisterVendor(
        VendorRegisterRequest request, 
        AuthService auth)
    {
        try 
        { 
            return Results.Ok(await auth.RegisterVendorOwnerAsync(request)); 
        }
        catch (Exception ex) 
        { 
            return Results.BadRequest(ex.Message); 
        }
    }

    [WolverinePost("/auth/vendor/login")]
    [Tags("Auth")]
    public static async Task<IResult> LoginVendor(
        LoginRequest request, 
        AuthService auth)
    {
        try 
        { 
            return Results.Ok(await auth.LoginVendorAsync(request)); 
        }
        catch (Exception ex) 
        { 
            return Results.Unauthorized(); 
        }
    }

    [WolverinePost("/auth/admin/login")]
    [Tags("Auth")]
    public static async Task<IResult> LoginAdmin(
        LoginRequest request, 
        AuthService auth)
    {
        try 
        { 
            return Results.Ok(await auth.LoginAdminAsync(request)); 
        }
        catch (Exception ex) 
        { 
            return Results.Unauthorized(); 
        }
    }

    [WolverinePost("/auth/vendor/users")]
    [Authorize] // Requires Valid Token
    [Tags("Auth")]
    public static async Task<IResult> CreateSubUser(
        CreateSubUserRequest request, 
        AuthService auth, 
        ClaimsPrincipal user)
    {
        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Results.Unauthorized();

        try 
        { 
            return Results.Ok(await auth.CreateSubUserAsync(userId, request)); 
        }
        catch (Exception ex) 
        { 
            return Results.BadRequest(ex.Message); 
        }
    }
}
