using OrderProcessing.Domain.Orders;

namespace OrderProcessing.Application.Orders.ChangeStatus;

public class ChangeOrderStatusHandler : IChangeOrderStatusHandler
{
    private readonly IOrderRepository _repository;

    public ChangeOrderStatusHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(ChangeOrderStatusCommand command)
    {
        var order = await _repository.GetByIdAsync(command.OrderId);

        if(order is null)
            throw new InvalidOperationException("Order not found");

        order.ChangeStatus(command.NewStatus);

        await _repository.SaveChangesAsync();
        
    }
}