using Testcontainers.PostgreSql;

public class PostgresFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; private set; } = null!;
    public string ConnectionString => Container.GetConnectionString();

    public async Task InitializeAsync()
    {
        var dbName = $"testdb_{Guid.NewGuid()}";

        Container = new PostgreSqlBuilder("postgres:15-alpine")
            .WithDatabase(dbName)
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await Container.StartAsync();
        Console.WriteLine($"PostgreSQL container started with connection string: {Container.GetConnectionString()}");
    }

    public async Task DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}