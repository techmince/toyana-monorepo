namespace Toyana.Contracts.Security;

public record struct VendorPermission
{
    public string Value { get; }

    private VendorPermission(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
             throw new ArgumentException("Permission cannot be empty", nameof(value));
        Value = value;
    }

    public static VendorPermission Create(string value) => new(value);

    // Predefined Permissions
    public static readonly VendorPermission UpdateProfile = new("VENDOR_UPDATE_PROFILE");
    public static readonly VendorPermission ManageServices = new("VENDOR_MANAGE_SERVICES");
    public static readonly VendorPermission ManageAvailability = new("VENDOR_MANAGE_AVAILABILITY");
    public static readonly VendorPermission ViewFinancials = new("VENDOR_VIEW_FINANCIALS");
    public static readonly VendorPermission ManageUsers = new("VENDOR_MANAGE_USERS");
    
    // Implicit conversion for ease of use in attributes/claims
    public static implicit operator string(VendorPermission permission) => permission.Value;
    public static implicit operator VendorPermission(string value) => new(value);
    
    public override string ToString() => Value;
}
