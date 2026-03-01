using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Api.Contracts.Orders;
using OrderProcessing.Application.Orders.Create;
using OrderProcessing.Application.Orders.List;
using OrderProcessing.Application.Orders.ChangeStatus;
using OrderProcessing.Domain.Orders;


namespace OrderProcessing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    #region Private Fields
    private readonly ICreateOrderHandler _createOrderHandler;
    private readonly IListOrderHandler _listOrderHandler;
    private readonly IChangeOrderStatusHandler _changeOrderStatusHandler;
    private readonly ILogger<OrdersController> _logger;
    #endregion

    public OrdersController(
        ICreateOrderHandler iCreateOrderHandler,
        IListOrderHandler iListOrderHandler,
        IChangeOrderStatusHandler iChangeOrderStatusHandler,
        ILogger<OrdersController> logger)
    {
        _createOrderHandler = iCreateOrderHandler;
        _listOrderHandler = iListOrderHandler;
        _changeOrderStatusHandler = iChangeOrderStatusHandler;
        _logger = logger;
    }
    

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        var command = new CreateOrderCommand(request.CustomerName);

        var orderId = await _createOrderHandler.Handle(command);

        _logger.LogInformation("Order created with ID: {OrderId}", orderId);

        return Ok(new { Id = orderId });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _listOrderHandler.Handle();

        return Ok(new { Orders = orders });     
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid id, 
        [FromBody] ChangeStatusRequest request)
    {     
        if (!Enum.TryParse<OrderStatus>(request.Status, true, out var status))
        return BadRequest("Invalid status value.");
                  
        var command = new ChangeOrderStatusCommand(id, status);

        await _changeOrderStatusHandler.Handle(command);

        _logger.LogInformation("Order with ID: {OrderId} status changed to {Status}", id, status);

        return NoContent();
    }
}