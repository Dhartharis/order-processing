using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;
using OrderProcessing.Infrastructure.Orders;
using OrderProcessing.Domain.Orders;

public class CreateOrderTests : IClassFixture<PostgresFixture>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CreateOrderTests(PostgresFixture fixture)
    {
        _factory = new CustomWebApplicationFactory(fixture.ConnectionString);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Should_Create_Order_And_Persist_In_Database()
    {
        // Arrange
        var request = new
        {
            customerName = "Hans"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", request);

        // Assert API
        response.EnsureSuccessStatusCode();

        // Assert DB
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

        var order = await db.Orders.FirstOrDefaultAsync();

        order.Should().NotBeNull();
        order!.CustomerName.Should().Be("Hans");
        order.Status.Should().Be(OrderStatus.Pending); // ajusta si cambia
        order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }
}