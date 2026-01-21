using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Toyana.Identity.Models;

public class ClientUser : AbstractUser
{
    [SetsRequiredMembers]
    public ClientUser(Guid id, string username, string phoneNumber, string password)
    {
        Id           = id;
        Username     = username;
        PhoneNumber  = phoneNumber;
        IsBanned     = false;
        CreatedAt    = DateTimeOffset.UtcNow;
        Salt         = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password + Salt);
    }

    [SetsRequiredMembers]
    public ClientUser(Guid id, string username, string phoneNumber, DateTimeOffset createdAt, bool isBanned, string passwordHash, string salt)
    {
        Id           = id;
        Username     = username;
        PhoneNumber  = phoneNumber;
        CreatedAt    = createdAt;
        IsBanned     = isBanned;
        PasswordHash = passwordHash;
        Salt         = salt;
    }

    [MaxLength(200)] public required string         Username    { get; set; }
    [MaxLength(200)] public required string         PhoneNumber { get; set; }
    public required                  DateTimeOffset CreatedAt   { get; set; }
    public required                  bool           IsBanned    { get; set; }

    public override string ToString()
    {
        return $"{Id}:{Username}:{PhoneNumber}";
    }
}