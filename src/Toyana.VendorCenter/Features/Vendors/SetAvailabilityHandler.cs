using Microsoft.EntityFrameworkCore;
using Toyana.Contracts;
using Toyana.VendorCenter.Data;
using Toyana.VendorCenter.Models;
using Wolverine;

namespace Toyana.VendorCenter.Features.Vendors;

public class SetAvailabilityHandler
{
    public static async Task Handle(SetAvailability command, VendorDbContext db, IMessageBus bus)
    {
        // Check if slot exists
        var existing = await db.AvailabilitySlots
            .FirstOrDefaultAsync(a => a.VendorId == command.VendorId && a.Date == command.Date);

        var statusEnum = Enum.Parse<AvailabilityStatus>(command.Status);

        if (existing != null)
        {
            existing.Status = statusEnum;
        }
        else
        {
            var slot = new Availability
            {
                Id = Guid.NewGuid(),
                VendorId = command.VendorId,
                Date = command.Date,
                Status = statusEnum
            };
            db.AvailabilitySlots.Add(slot);
        }

        await db.SaveChangesAsync();

        await bus.PublishAsync(new AvailabilityUpdated(command.VendorId, command.Date, command.Status));
    }
}
