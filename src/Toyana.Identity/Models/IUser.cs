using System.ComponentModel.DataAnnotations;

namespace Toyana.Identity.Models;

public interface IUser
{
    [MaxLength(200)] public string PasswordHash { get; }
    [MaxLength(200)] public string Salt         { get; }
    bool                           VerifyPassword(string pass);
}