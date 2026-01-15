using Wolverine;
using Toyana.Contracts;
using Toyana.VendorCenter.Data;
using Toyana.VendorCenter.Models;

namespace Toyana.VendorCenter.Features.Vendors;

public class AddServiceHandler
{
    public static async Task Handle(AddService command, VendorDbContext db, IMessageBus bus)
    {
        var service = new Service
        {
            Id = Guid.NewGuid(),
            VendorId = command.VendorId,
            Name = command.Name,
            Description = command.Description,
            Price = command.Price
        };

        db.Services.Add(service);
        await db.SaveChangesAsync();

        await bus.PublishAsync(new ServiceAdded(service.Id, command.VendorId, command.Name, command.Description, command.Price));
    }
}
