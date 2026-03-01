using System.Security.Claims;
using SamplePaymentsForOrders.Services.Abstractions;

namespace SamplePaymentsForOrders.Services;

public class AuthenticatedService(IHttpContextAccessor httpContextAccessor) : IAuthenticatedService
{

    public Guid UserId => Guid.Parse(httpContextAccessor
        .HttpContext?.User
        .Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
}