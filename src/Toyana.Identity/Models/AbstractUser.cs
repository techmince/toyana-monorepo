using System.ComponentModel.DataAnnotations;
using Toyana.Shared.Entity;

namespace Toyana.Identity.Models;

public abstract class AbstractUser : Entity, IUser
{
    [MaxLength(200)] public required string PasswordHash { get; set; }
    [MaxLength(200)] public required string Salt         { get; set; }

    public bool VerifyPassword(string pass)
    {
        return BCrypt.Net.BCrypt.Verify(pass + Salt, PasswordHash);
    }
}