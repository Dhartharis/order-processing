using OrderProcessing.Domain.Orders;
namespace OrderProcessing.Application.Orders.List;

public interface IListOrderHandler
{
    Task<List<Order>> Handle();
}