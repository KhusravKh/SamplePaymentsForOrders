using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SamplePaymentsForOrders.Common;
using SamplePaymentsForOrders.Contexts;
using SamplePaymentsForOrders.Dtos.Auth;
using SamplePaymentsForOrders.Services.Abstractions;

namespace SamplePaymentsForOrders.Services;

public class AuthenticationService(
    IDateTimeService dateTimeService,
    ApplicationDbContext dbContext,
    IOptions<JwtConfig> jwtConfigOption) : IAuthenticationService
{
    private readonly JwtConfig _jwtConfig = jwtConfigOption.Value;
    private const short MaxAttempts = 5;
    
    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Login.ToLower() == loginRequestDto.Login.ToLower());
        
        if (user == null)
        {
            throw new AppLogicException(ExceptionStatus.BadRequest, "Invalid login or password");
        }

        if (user.LockEndTime != null && user.LockEndTime > dateTimeService.UtcNow)
        {
            throw new AppLogicException(ExceptionStatus.BadRequest, "You have to many attempts. Try again later");
        }
        
        var passwordHas = Convert.ToBase64String(Encoding.UTF8.GetBytes(loginRequestDto.Password));
        if (passwordHas != user.PasswordHash)
        {
            user.Attempts++;
            if (user.Attempts >= MaxAttempts)
            {
                user.LockEndTime = dateTimeService.UtcNow.AddMinutes(30);
                user.Attempts = 0;
            }
            await dbContext.SaveChangesAsync();
            throw new AppLogicException(ExceptionStatus.BadRequest, "Invalid login or password");
        }
        
        user.Attempts = 0;
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: dateTimeService.Now.AddMinutes(_jwtConfig.ExpireInMinutes),
            signingCredentials: credentials);
        
        await dbContext.SaveChangesAsync();
        return new LoginResponseDto(new JwtSecurityTokenHandler().WriteToken(token));
    }
}