using Marten;
using Wolverine.Http;
using Toyana.Catalog.Models;

namespace Toyana.Catalog.Features.Search;

public static class SearchEndpoints
{
    [WolverineGet("/catalog/search")]
    [Tags("Search")]
    public static async Task<IReadOnlyList<VendorReadModel>> SearchVendors(
        IQuerySession session, 
        string? category, 
        string? date)
    {
        // Basic query
        var query = session.Query<VendorReadModel>();

        if (!string.IsNullOrEmpty(category))
        {
            query = (Marten.Linq.IMartenQueryable<VendorReadModel>)query.Where(v => v.Category == category);
        }

        if (!string.IsNullOrEmpty(date) && DateOnly.TryParse(date, out var dateOnly))
        {
            // Must NOT contain the date in BlockedDates
            query = (Marten.Linq.IMartenQueryable<VendorReadModel>)query.Where(v => !v.BlockedDates.Contains(dateOnly));
        }

        return await query.ToListAsync();
    }

    [WolverineGet("/catalog/vendors/{id}")]
    [Tags("Search")]
    public static async Task<IResult> GetVendor(
        Guid id, 
        IQuerySession session)
    {
        var vendor = await session.LoadAsync<VendorReadModel>(id);
        return vendor is null ? Results.NotFound() : Results.Ok(vendor);
    }

    [WolverineGet("/catalog/featured")]
    [Tags("Search")]
    public static async Task<IReadOnlyList<VendorReadModel>> GetFeaturedVendors(
        IQuerySession session)
    {
        // Mocking "Featured" by taking top 3. 
        // In future, sort by Rating if available.
        return await session.Query<VendorReadModel>()
            .Take(3)
            .ToListAsync();
    }
}
