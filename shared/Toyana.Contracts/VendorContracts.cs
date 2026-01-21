namespace Toyana.Contracts;

// Events
public record VendorCreated(Guid VendorId, string BusinessName, string TaxId, string Category);

public record ServiceAdded(Guid ServiceId, Guid VendorId, string Name, string Description, decimal Price);

public record AvailabilityUpdated(Guid VendorId, DateOnly Date, string Status); // Status: Available, Blocked

// Commands
public record CreateVendor(string BusinessName, string TaxId, string Category);

public record AddService(Guid VendorId, string Name, string Description, decimal Price);

public record SetAvailability(Guid VendorId, DateOnly Date, string Status);