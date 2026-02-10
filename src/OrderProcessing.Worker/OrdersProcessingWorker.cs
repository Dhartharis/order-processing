
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Domain.Orders;
using OrderProcessing.Infrastructure.Orders;

public class OrdersProcessingWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrdersProcessingWorker> _logger;

    public OrdersProcessingWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<OrdersProcessingWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

                var order = await db.Orders
                    .Where(o => o.Status == OrderStatus.Pending)
                    .OrderBy(o => o.CreatedAt)
                    .FirstOrDefaultAsync(stoppingToken);

                if (order is null) continue;                

                order.ChangeStatus(OrderStatus.Processing);
                await db.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("Processing order {OrderId}", order.Id);

                await Task.Delay(2000, stoppingToken); // simulate work

                order.ChangeStatus(OrderStatus.Completed);
                await db.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("Order {OrderId} completed", order.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Worker loop failed");
                await Task.Delay(5000, stoppingToken);
            }

            await Task.Delay(3000, stoppingToken);
        }
    }
}