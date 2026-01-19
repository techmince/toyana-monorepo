using System.ComponentModel.DataAnnotations;

namespace Toyana.Identity.Models;

public class AdminUser : AbstractUser
{
    [MaxLength(300)] public required string Username { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}