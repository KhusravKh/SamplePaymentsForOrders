using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SamplePaymentsForOrders.Dtos.Payment;
using SamplePaymentsForOrders.Services.Abstractions;

namespace SamplePaymentsForOrders.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
[EnableRateLimiting("user-id")]
public class PaymentsController(IPaymentService paymentService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequestDto createPaymentRequestDto, CancellationToken cancellationToken)
    {
        var response = await paymentService.Create(createPaymentRequestDto, cancellationToken);
        return Ok(response);
    }

    [HttpPost("confirm/{paymentId:guid}")]
    public async Task<IActionResult> Confirm([FromRoute] Guid paymentId, CancellationToken cancellationToken)
    {
        await paymentService.ConfirmPaymentAsync(paymentId, cancellationToken);
        return Ok();
    }

    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrderId([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var payments = await paymentService.GetPaymentByOrderIdAsync(orderId, cancellationToken);
        return Ok(payments);
    }
}