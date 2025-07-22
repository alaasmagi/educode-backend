namespace Contracts;

public interface IOtpService
{
    Task<bool> GenerateAndStoreOtp(Guid userId);
    Task<bool> VerifyOtp(Guid userId, string otpToVerify);
}