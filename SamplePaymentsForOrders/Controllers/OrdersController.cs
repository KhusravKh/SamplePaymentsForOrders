using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SamplePaymentsForOrders.Dtos.Order;
using SamplePaymentsForOrders.Services.Abstractions;

namespace SamplePaymentsForOrders.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[EnableRateLimiting("user-id")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto createOrderRequestDto, CancellationToken cancellationToken)
    {
        var createOrderResponseDto = await orderService.CreateOrderAsync(createOrderRequestDto, cancellationToken);
        return Ok(createOrderResponseDto);
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetOrder(Guid orderId, CancellationToken cancellationToken)
    {
        var orderResponseDto = await orderService.GetOrderAsync(orderId, cancellationToken);
        return Ok(orderResponseDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
    {
        var orders = await orderService.GetOrdersAsync(cancellationToken);
        return Ok(orders);
    }
}