namespace OrderProcessing.Application.Orders.Create;

public interface ICreateOrderHandler
{
    Task<Guid> Handle(CreateOrderCommand command);
}