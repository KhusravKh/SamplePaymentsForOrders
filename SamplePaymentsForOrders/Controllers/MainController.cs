using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SamplePaymentsForOrders.Services.Abstractions;

namespace SamplePaymentsForOrders.Controllers;

[ApiController]
[Route("[controller]")]
[EnableRateLimiting("user-id")]
public class MainController(IAuthenticatedService authenticatedService) : ControllerBase
{
    [HttpGet("get-user-data")]
    [Authorize]
    public IActionResult GetUserId()
    {
        var userId = authenticatedService.UserId;
        return Ok(new { UserId = userId, Role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value });
    }
}