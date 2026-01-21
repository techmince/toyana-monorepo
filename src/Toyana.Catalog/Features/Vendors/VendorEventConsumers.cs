using Marten;
using Toyana.Catalog.Models;
using Toyana.Contracts;

namespace Toyana.Catalog.Features.Vendors;

public class VendorEventConsumers
{
    // Create new Vendor Document
    public static void Consume(VendorCreated @event, IDocumentSession session)
    {
        var model = new VendorReadModel
                    {
                        Id           = @event.VendorId,
                        BusinessName = @event.BusinessName,
                        Category     = @event.Category
                    };
        session.Store(model);
    }

    // Update Services
    public static async Task Consume(ServiceAdded @event, IDocumentSession session)
    {
        var vendor = await session.LoadAsync<VendorReadModel>(@event.VendorId);
        if (vendor != null)
        {
            vendor.Services.Add(new ServiceView
                                {
                                    ServiceId   = @event.ServiceId,
                                    Name        = @event.Name,
                                    Description = @event.Description,
                                    Price       = @event.Price
                                });
            session.Store(vendor);
        }
    }

    // Update Availability
    public static async Task Consume(AvailabilityUpdated @event, IDocumentSession session)
    {
        var vendor = await session.LoadAsync<VendorReadModel>(@event.VendorId);
        if (vendor == null) return;

        // "Blocked" or "Booked" -> Add to BlockedDates
        // "Available" -> Remove from BlockedDates
        var isBlocked = @event.Status == "Blocked" || @event.Status == "Booked";

        if (isBlocked)
        {
            if (!vendor.BlockedDates.Contains(@event.Date)) vendor.BlockedDates.Add(@event.Date);
        }
        else
        {
            vendor.BlockedDates.Remove(@event.Date);
        }

        session.Store(vendor);
    }
}