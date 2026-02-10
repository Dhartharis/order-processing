using OrderProcessing.Domain.Orders;

namespace OrderProcessing.Application.Orders.List;

public class ListOrderHandler
{
    private readonly IOrderRepository _repository;

    public ListOrderHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Order>> Handle()
    {
        return _repository.GetAllAsync();
    }
}