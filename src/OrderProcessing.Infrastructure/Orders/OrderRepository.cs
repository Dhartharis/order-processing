using Microsoft.EntityFrameworkCore;
using OrderProcessing.Domain.Orders;

namespace OrderProcessing.Infrastructure.Orders;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _db;

    public OrderRepository(OrderDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Order order)
    {
        await _db.Orders.AddAsync(order);   
    }

    public Task<Order?> GetByIdAsync(Guid id)
    {
        return _db.Orders.FirstOrDefaultAsync(o => o.Id == id);
    }

    public Task<List<Order>> GetAllAsync()
    {
        return _db.Orders.ToListAsync();
    }

    public Task SaveChangesAsync()
    {
        return _db.SaveChangesAsync();
    }
}