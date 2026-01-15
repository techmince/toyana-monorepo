using Marten.Schema;

namespace Toyana.Catalog.Models;

public class VendorReadModel
{
    // Marten uses Id as the Primary Key (PK)
    public Guid Id { get; set; }
    public string BusinessName { get; set; }
    public string Category { get; set; }
    public List<ServiceView> Services { get; set; } = new();
    
    // For efficient querying "Is Available on UseRequestedDate?"
    // We store ONLY the blocked dates. Queries will check if RequestedDate is NOT in this list.
    public List<DateOnly> BlockedDates { get; set; } = new();
}

public class ServiceView
{
    public Guid ServiceId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}
