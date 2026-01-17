using Marten;
using Microsoft.AspNetCore.Authorization;
using Wolverine.Http;

namespace Toyana.Admin.Features.Payments;

public static class PaymentsEndpoints
{
    [WolverineGet("/admin/payments")]
    [Authorize(Roles = "Admin")]
    [Tags("Admin")]
    public static async Task<IResult> ListPayments(IQuerySession session)
    {
        using var conn = session.Connection;
        if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
        
        using var command = conn.CreateCommand();
        command.CommandText = "SELECT data FROM payments.mt_doc_payment"; // Assuming Schema
        
        // Note: In real app, check schema name
        try 
        {
                using var reader = await command.ExecuteReaderAsync();
                var payments = new List<string>();
                while (await reader.ReadAsync())
                {
                    payments.Add(reader.GetString(0)); // Returns JSON string
                }
                return Results.Content($"[{string.Join(",", payments)}]", "application/json");
        }
        catch(Exception ex)
        {
            // Fallback if table doesn't exist yet
            // return Results.Ok(new List<object>());
            throw;
        }
    }
}
