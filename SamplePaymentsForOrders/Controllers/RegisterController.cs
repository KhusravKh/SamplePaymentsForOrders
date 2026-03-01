using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SamplePaymentsForOrders.Dtos.Register;
using SamplePaymentsForOrders.Services.Abstractions;

namespace SamplePaymentsForOrders.Controllers;

[ApiController]
[Route("[controller]")]
[EnableRateLimiting("user-ip")]
public class RegisterController(IRegisterUserService registerUserService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto registerUserRequestDto,
        CancellationToken cancellationToken)
    {
        var registerUserResponseDto =
            await registerUserService.RegisterUser(registerUserRequestDto, cancellationToken);
        return Ok(registerUserResponseDto);
    }

    [HttpPost("otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpCodeRequestDto sendOtpCodeRequestDto, CancellationToken cancellationToken)
    {
        await registerUserService.SendOtpCode(sendOtpCodeRequestDto, cancellationToken);
        return Ok();
    }

    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCodeRequestDto resendOtpCodeRequestDto,
        CancellationToken cancellationToken)
    {
        await registerUserService.ResendOtpCode(resendOtpCodeRequestDto, cancellationToken);
        return Ok();
    }
}