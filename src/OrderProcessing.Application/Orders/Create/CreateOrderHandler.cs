using OrderProcessing.Domain.Orders;

namespace OrderProcessing.Application.Orders.Create;

public class CreateOrderHandler: ICreateOrderHandler
{
    private readonly IOrderRepository _repository;

    public CreateOrderHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateOrderCommand command)
    {
        var order = new Order(command.CustomerName);
        
        await _repository.AddAsync(order);
        await _repository.SaveChangesAsync();

        return order.Id;
    }
}