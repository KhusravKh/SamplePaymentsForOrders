using SamplePaymentsForOrders.Dtos.Auth;

namespace SamplePaymentsForOrders.Services.Abstractions;

public interface IAuthenticationService
{
    Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
}