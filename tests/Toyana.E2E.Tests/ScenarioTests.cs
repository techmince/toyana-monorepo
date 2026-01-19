using System.Net;
using FluentAssertions;
using RestSharp;

namespace Toyana.E2E.Tests;

[Collection("ToyanaE2E")]
public class ScenarioTests
{
    private readonly ToyanaApiFixture _fixture;
    private readonly RestClient _client;

    public ScenarioTests(ToyanaApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
    }

    [Fact]
    public async Task HealthCheck_ShouldReturnOk()
    {
        // Simple reachable check (e.g. Identity Auth endpoint without credentials -> 401 or 400).
        var request = new RestRequest("/api/identity/auth/admin/login", Method.Post);
        var response = await _client.ExecuteAsync(request);
        // Expecting 400 Bad Request (missing body) or 401 Unauthorized, but reachable.
        // If 502/503/404, then Gateway issue.
        response.StatusCode.Should().NotBe(HttpStatusCode.BadGateway);
        response.StatusCode.Should().NotBe(HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task FullWorkflow_VendorLifecycle_And_Booking()
    {
        // 1. Authenticate as Admin
        var token = await AuthenticateAdmin();
        token.Should().NotBeNullOrEmpty("Admin should receive an access token");

        // 2. Register a new Vendor (Vendor API via Gateway)
        var vendorId = Guid.NewGuid();
        var vendorRequest = new 
        { 
            BusinessName = $"E2E Vendor {DateTime.UtcNow.Ticks}", 
            TaxId = $"TAX-{DateTime.UtcNow.Ticks}",
            LegalType = "LLC",
            ContactEmail = "vendor@test.com",
            PhoneNumber = "+1234567890"
        };

        var vendorReq = new RestRequest("/api/vendor/vendors", Method.Post);
        vendorReq.AddHeader("Authorization", $"Bearer {token}");
        vendorReq.AddJsonBody(vendorRequest);
        
        var vendorResponse = await _client.ExecuteAsync<VendorResponse>(vendorReq);
        vendorResponse.StatusCode.Should().Be(HttpStatusCode.Created, $"Vendor creation failed: {vendorResponse.Content}"); 
        vendorResponse.Data.Should().NotBeNull();
        var createdVendorId = vendorResponse.Data.Id;

        // 3. Verify Vendor in VendorCenter
        var getVendorReq = new RestRequest($"/api/vendor/vendors/{createdVendorId}", Method.Get);
        getVendorReq.AddHeader("Authorization", $"Bearer {token}");
        var getVendorRes = await _client.ExecuteAsync<VendorResponse>(getVendorReq);
        getVendorRes.IsSuccessful.Should().BeTrue();
        getVendorRes.Data.BusinessName.Should().Be(vendorRequest.BusinessName);

        // 4. Verify Vendor in Catalog (Eventual Consistency)
        VendorCatalogItem? catalogVendor = null;
        for (int i = 0; i < 15; i++) // Poll up to 7.5s
        {
            var searchReq = new RestRequest("/api/catalog/search", Method.Get);
            searchReq.AddQueryParameter("q", vendorRequest.BusinessName);
            
            var searchRes = await _client.ExecuteAsync<CatalogSearchResult>(searchReq);
            if (searchRes.IsSuccessful && searchRes.Data != null && searchRes.Data.Items.Any())
            {
                catalogVendor = searchRes.Data.Items.FirstOrDefault(v => v.BusinessName == vendorRequest.BusinessName);
                if (catalogVendor != null) break;
            }
            await Task.Delay(500);
        }

        catalogVendor.Should().NotBeNull("Vendor should appear in Catalog (read model) after synchronization");

        // 5. Create a Booking (Ordering)
        var bookingReq = new RestRequest("/api/ordering/bookings", Method.Post);
        bookingReq.AddHeader("Authorization", $"Bearer {token}");
        
        var bookingBody = new 
        {
            VendorId = createdVendorId,
            Amount = 1000.00m,
            EventDate = DateTime.UtcNow.AddDays(10)
        };
        bookingReq.AddJsonBody(bookingBody);

        var bookingRes = await _client.ExecuteAsync<BookingResponse>(bookingReq);
        bookingRes.StatusCode.Should().Be(HttpStatusCode.Created, $"Booking creation failed: {bookingRes.Content}");
        bookingRes.Data.Should().NotBeNull();
        bookingRes.Data.Status.Should().Be("PendingVendorApproval");
    }

    private async Task<string> AuthenticateAdmin()
    {
        var request = new RestRequest("/api/identity/auth/admin/login", Method.Post);
        request.AddJsonBody(new 
        { 
            Username = "admin", 
            Password = "admin123"
        });

        var response = await _client.ExecuteAsync<LoginResponse>(request);
        if (!response.IsSuccessful) 
        {
            return null;
        }
        return response.Data?.AccessToken;
    }

    // DTOs
    record LoginResponse(string AccessToken, string RefreshToken);
    record VendorResponse(Guid Id, string BusinessName, string TaxId);
    record CatalogSearchResult(List<VendorCatalogItem> Items, int TotalCount);
    record VendorCatalogItem(Guid Id, string BusinessName);
    record BookingResponse(Guid Id, string Status, decimal Amount);
}
