namespace OrderProcessing.Domain.Orders;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(Guid id);
    Task SaveChangesAsync();
}