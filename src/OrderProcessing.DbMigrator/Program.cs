using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OrderProcessing.Infrastructure.Orders;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("Default");

        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(connectionString));
    })
    .Build();

var configuration = host.Services.GetRequiredService<IConfiguration>();
var connectionString = configuration.GetConnectionString("Default")
    ?? throw new Exception("Connection string 'Default' not found.");

var csBuilder = new NpgsqlConnectionStringBuilder(connectionString);

var databaseName = csBuilder.Database;

// Nos conectamos a postgres para crear la DB real
csBuilder.Database = "postgres";

await using (var conn = new NpgsqlConnection(csBuilder.ConnectionString))
{
    await conn.OpenAsync();

    await using var cmd = new NpgsqlCommand(
        "SELECT 1 FROM pg_database WHERE datname = @name",
        conn);

    cmd.Parameters.AddWithValue("name", databaseName);

    var exists = await cmd.ExecuteScalarAsync();

    if (exists == null)
    {
        await using var createCmd =
            new NpgsqlCommand($"""CREATE DATABASE "{databaseName}" """, conn);

        await createCmd.ExecuteNonQueryAsync();

        Console.WriteLine($"Database '{databaseName}' created.");
    }
    else
    {
        Console.WriteLine($"Database '{databaseName}' already exists.");
    }
}

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

Console.WriteLine("Applying migrations...");

await db.Database.MigrateAsync();

Console.WriteLine("Database migration completed successfully.");
