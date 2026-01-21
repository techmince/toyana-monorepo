using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Toyana.VendorCenter.Models;

public class Vendor
{
    public Guid Id { get; set; }

    [MaxLength(300)] public string BusinessName { get; set; } = string.Empty;

    [MaxLength(300)] public string TaxId { get; set; } = string.Empty;

    [MaxLength(300)] public string Category { get; set; } = string.Empty; // e.g., "Videography", "Catering"

    [MaxLength(300)] public string Description { get; set; } = string.Empty;

    public bool IsVerified { get; set; }

    public List<Service>      Services          { get; set; } = new();
    public List<Availability> AvailabilitySlots { get; set; } = new();
}

public class Service
{
    public Guid Id       { get; set; }
    public Guid VendorId { get; set; }

    [MaxLength(300)] public string Name { get; set; } = string.Empty;

    [MaxLength(300)] public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    [JsonIgnore] public Vendor Vendor { get; set; } = null!;
}

public class Availability
{
    public Guid               Id       { get; set; }
    public Guid               VendorId { get; set; }
    public DateOnly           Date     { get; set; }
    public AvailabilityStatus Status   { get; set; } // Available, Blocked

    [JsonIgnore] public Vendor Vendor { get; set; } = null!;
}

public enum AvailabilityStatus
{
    Available,
    Blocked,
    Booked
}