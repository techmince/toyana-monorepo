using Toyana.Identity.Models;

namespace Toyana.Identity.Domain.Repositories;

public interface IVendorUserRepository
{
    Task<VendorUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<VendorUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<VendorUser?> GetByIdWithRolesAndPermissionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool>        ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task              AddAsync(VendorUser user, CancellationToken cancellationToken = default);
    void              Update(VendorUser user);
}