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

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateOrderRequest request,
        [FromServices] CreateOrderHandler _createOrderHandler)
    {
        var command = new CreateOrderCommand(request.CustomerName);

        var orderId = await _createOrderHandler.Handle(command);

        return Ok(new { Id = orderId });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromServices] ListOrderHandler _listOrderHandler)
    {
        var orders = await _listOrderHandler.Handle();

        return Ok(new { Orders = orders });     
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid id, 
        [FromBody] ChangeStatusRequest request,
        [FromServices] ChangeOrderStatusHandler _changeOrderStatusHandler)
    {     
        if (!Enum.TryParse<OrderStatus>(request.Status, true, out var status))
        return BadRequest("Invalid status value.");
                  
        var command = new ChangeOrderStatusCommand(id, status);

        await _changeOrderStatusHandler.Handle(command);

        return NoContent();
    }
}