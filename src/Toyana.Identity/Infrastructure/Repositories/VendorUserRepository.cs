using Microsoft.EntityFrameworkCore;
using Toyana.Identity.Data;
using Toyana.Identity.Domain.Repositories;
using Toyana.Identity.Models;

namespace Toyana.Identity.Infrastructure.Repositories;

public class VendorUserRepository : IVendorUserRepository
{
    private readonly ApplicationDbContext _context;

    public VendorUserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VendorUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.VendorUsers.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<VendorUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.VendorUsers
                             .SingleOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<VendorUser?> GetByIdWithRolesAndPermissionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.VendorUsers
                             .Include(u => u.VendorOrganization)
                             .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.VendorUsers.AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task AddAsync(VendorUser user, CancellationToken cancellationToken = default)
    {
        await _context.VendorUsers.AddAsync(user, cancellationToken);
    }

    public void Update(VendorUser user)
    {
        _context.VendorUsers.Update(user);
    }
}