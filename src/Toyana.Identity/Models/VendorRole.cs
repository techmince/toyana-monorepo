using System.ComponentModel.DataAnnotations;
using Toyana.Shared.Entity;

namespace Toyana.Identity.Models;

public class VendorRole : Entity
{
    public required Guid VendorId { get; set; }
    [MaxLength(200)] public required string Name { get; set; }
    public List<Permission> Permissions { get; set; } = new();
}

public class Permission : Entity
{
    [MaxLength(4000)]
    public string Scope { get; set; } = string.Empty;
    
    [MaxLength(4000)]
    public string Action { get; set; } = string.Empty;
}