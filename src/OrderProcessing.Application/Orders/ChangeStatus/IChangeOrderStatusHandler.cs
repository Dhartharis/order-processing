namespace OrderProcessing.Application.Orders.ChangeStatus;

public interface IChangeOrderStatusHandler
{
    Task Handle(ChangeOrderStatusCommand command);
}