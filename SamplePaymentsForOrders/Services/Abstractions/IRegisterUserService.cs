using SamplePaymentsForOrders.Dtos.Register;

namespace SamplePaymentsForOrders.Services.Abstractions;

public interface IRegisterUserService
{
    Task<RegisterUserResponseDto> RegisterUser(RegisterUserRequestDto request, CancellationToken cancellationToken);
    
    Task SendOtpCode(SendOtpCodeRequestDto request, CancellationToken cancellationToken);
    
    Task ResendOtpCode(ResendOtpCodeRequestDto request, CancellationToken cancellationToken);
}