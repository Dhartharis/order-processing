using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessing.Application.Orders;
using OrderProcessing.Infrastructure.Orders;

namespace OrderProcessing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            var host = configuration["DB_HOST"];
            var db   = configuration["DB_NAME"];
            var user = configuration["DB_USER"];
            var pass = configuration["DB_PASSWORD"];

            connectionString = $"Host={host};Port=5432;Database={db};Username={user};Password={pass}";
        }
        services.AddDbContext<OrderDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}