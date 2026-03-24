using Moq;
using FluentAssertions;
using Xunit;
using OrderProcessing.Application.Orders;        // ← IOrderRepository
using OrderProcessing.Application.Orders.Create; // ← Handler + Command
using OrderProcessing.Domain.Orders;


public class CreateOrderHandlerTests
{
    [Fact]
    public async Task Should_Capture_Created_Order()
    {
        // Arrange
        var repoMock = new Mock<IOrderRepository>();

        Order? capturedOrder = null;

        repoMock
            .Setup(r => r.AddAsync(It.IsAny<Order>()))
            .Callback<Order>(o => capturedOrder = o)
            .Returns(Task.CompletedTask);

        var handler = new CreateOrderHandler(repoMock.Object);
        var command = new CreateOrderCommand("Hans");

        // Act
        await handler.Handle(command);

        // Assert
        capturedOrder.Should().NotBeNull();
        capturedOrder!.CustomerName.Should().Be("Hans");
        capturedOrder.Status.Should().Be(OrderStatus.Pending);
        capturedOrder.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
   
    }

    [Fact]
    public async Task Should_Throw_When_CustomerName_Is_Empty()
    {
        // Arrange
        var repoMock = new Mock<IOrderRepository>();
        var handler = new CreateOrderHandler(repoMock.Object);

        var command = new CreateOrderCommand("");

        // Act
        Func<Task> act = async () => await handler.Handle(command);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}