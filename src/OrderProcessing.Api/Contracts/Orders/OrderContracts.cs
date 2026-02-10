namespace OrderProcessing.Api.Contracts.Orders;

public class OrderContracts
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}