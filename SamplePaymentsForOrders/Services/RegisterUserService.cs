using System.Security.Cryptography;
using System.Text;
using Medallion.Threading;
using Microsoft.EntityFrameworkCore;
using SamplePaymentsForOrders.Common;
using SamplePaymentsForOrders.Contexts;
using SamplePaymentsForOrders.Dtos.Register;
using SamplePaymentsForOrders.Models;
using SamplePaymentsForOrders.Models.Enums;
using SamplePaymentsForOrders.Services.Abstractions;

namespace SamplePaymentsForOrders.Services;

public class RegisterUserService(
    ApplicationDbContext dbContext,
    IDateTimeService dateTimeService,
    IDistributedLockProvider distributedLockProvider) : IRegisterUserService
{
    private const short MaxAttempt = 5;
    private const short MaxTryIn30Minutes = 3;
    public async Task<RegisterUserResponseDto> RegisterUser(RegisterUserRequestDto request, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        var otpCode = await dbContext.OtpCodes
                          .FromSqlRaw("SELECT * FROM \"OtpCodes\" FOR UPDATE")
                          .FirstOrDefaultAsync(o => o.PhoneNumber == request.PhoneNumber 
                                      && o.ExpiresAt > dateTimeService.UtcNow 
                                      && o.Attempts < MaxAttempt && !o.IsUsed, cancellationToken)
                      ?? throw new AppLogicException(ExceptionStatus.NotFound, "Otp code not found");
        
        if (otpCode.Code != request.OtpCode)
        {
            otpCode.Attempts++;
            otpCode.LockEndTime = dateTimeService.UtcNow.AddMinutes(30);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            throw new AppLogicException(ExceptionStatus.BadRequest, "Invalid otp code");
        }
        
        otpCode.IsUsed = true;
        
        var passwordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Password));
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Login = request.Login,
            PasswordHash = passwordHash,
            Role = UserRoles.User
        };
        
        await  dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        
        return new RegisterUserResponseDto(user.Id, user.Role.ToString());
    }

    public async Task SendOtpCode(SendOtpCodeRequestDto request, CancellationToken cancellationToken)
    {
        await using var distributedLock = await distributedLockProvider
            .TryAcquireLockAsync($"send-otp-code-lock-{request.PhoneNumber}", cancellationToken: cancellationToken);

        if (distributedLock == null)
        {
            throw new AppLogicException(ExceptionStatus.Conflict, "For ths phone number already sending otp code is in process.");
        }
        
        var otpCodes = await dbContext.OtpCodes
            .Where(o => o.PhoneNumber == request.PhoneNumber 
                        && o.CreatedAt < dateTimeService.UtcNow.AddMinutes(30))
            .ToListAsync(cancellationToken);
        
        var isAlreadySent = otpCodes    
            .Any(o =>
                           o.ExpiresAt > dateTimeService.UtcNow 
                           && o is { Attempts: < MaxAttempt, IsUsed: false });
        if (isAlreadySent)
        {
            throw new AppLogicException(ExceptionStatus.BadRequest, "Otp code already sent");
        }
        
        var isLocked = otpCodes
            .Any(o => o.LockEndTime != null && o.LockEndTime > dateTimeService.UtcNow);
        
        if (isLocked || otpCodes.Count >= MaxTryIn30Minutes)
        {
            throw new AppLogicException(ExceptionStatus.BadRequest, "You have to many attempts. Try again later");
        }

        var otpCode = new OtpCode
        {
            Id = Guid.NewGuid(),
            PhoneNumber = request.PhoneNumber,
            Code = RandomNumberGenerator.GetInt32(100000, 999999).ToString(),
            ExpiresAt = dateTimeService.UtcNow.AddMinutes(5),
            Attempts = 0,
            IsUsed = false,
            CodeSendAt = dateTimeService.UtcNow
        };
        
        //TODO send otp code to phone number
        
        await dbContext.OtpCodes.AddAsync(otpCode, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ResendOtpCode(ResendOtpCodeRequestDto request, CancellationToken cancellationToken)
    {
        var otpCode = await dbContext.OtpCodes
            .FirstOrDefaultAsync(o => o.PhoneNumber == request.PhoneNumber 
                                      && o.ExpiresAt > dateTimeService.UtcNow 
                                      && o.Attempts < MaxAttempt && !o.IsUsed 
                                      && o.CodeSendAt > dateTimeService.UtcNow.AddMinutes(1), cancellationToken)
                      ?? throw  new AppLogicException(ExceptionStatus.BadRequest, "Otp code not found or you can resend otp code after 1 minutes");
        
        otpCode.CodeSendAt = dateTimeService.UtcNow;
        
        //TODO send otp code to phone number
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}