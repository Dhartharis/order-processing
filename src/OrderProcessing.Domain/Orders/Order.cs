namespace OrderProcessing.Domain.Orders;

public class Order
{
    public Guid Id{get; private set;}
    public string CustomerName { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Order(){} //EF Core

    public Order(string CustomerName)
    {
        Id = Guid.NewGuid();
        this.CustomerName = CustomerName;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        Status = newStatus;
    }
}
