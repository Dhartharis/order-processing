using OrderProcessing.Domain.Orders;
namespace OrderProcessing.Application.Orders.ChangeStatus;

public record ChangeOrderStatusCommand(Guid OrderId, OrderStatus NewStatus);