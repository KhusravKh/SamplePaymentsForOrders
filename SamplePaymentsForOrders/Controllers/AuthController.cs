using Microsoft.AspNetCore.Mvc;
using SamplePaymentsForOrders.Dtos.Auth;
using SamplePaymentsForOrders.Services.Abstractions;

namespace SamplePaymentsForOrders.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        var loginResponseDto = await authenticationService.Login(loginRequestDto);
        return Ok(loginResponseDto);
    }
}