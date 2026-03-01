namespace SamplePaymentsForOrders.Dtos.Register;

public record RegisterUserRequestDto(string FullName, string PhoneNumber, string Login, string Password, string OtpCode);