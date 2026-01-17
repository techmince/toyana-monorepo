using Toyana.Contracts;
using Toyana.VendorCenter.Data;
using Toyana.VendorCenter.Models;
using Wolverine;

namespace Toyana.VendorCenter.Features.Vendors;

public class CreateVendorHandler
{
    // Wolverine injects DbContext automatically if registered
    public static async Task Handle(CreateVendor command, VendorDbContext db, IMessageBus bus)
    {
        var vendor = new Vendor
        {
            Id = Guid.NewGuid(),
            BusinessName = command.BusinessName,
            TaxId = command.TaxId,
            Category = command.Category,
            IsVerified = false // Pending verification
        };

        db.Vendors.Add(vendor);
        await db.SaveChangesAsync();

        // Publish event for Catalog Service
        await bus.PublishAsync(new VendorCreated(vendor.Id, vendor.BusinessName, vendor.TaxId, vendor.Category));
    }
}
