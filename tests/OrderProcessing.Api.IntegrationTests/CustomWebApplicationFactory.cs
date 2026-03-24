using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Infrastructure.Orders;
using System.Linq;

public class CustomWebApplicationFactory 
    : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public CustomWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remover DbContext existente
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<OrderDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            // Registrar PostgreSQL real
            services.AddDbContext<OrderDbContext>(options =>
                options.UseNpgsql(_connectionString));

            // 🔥 Aplicar migraciones
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            db.Database.Migrate();
        });
    }
}