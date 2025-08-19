namespace Contracts;

public interface IOtpService
{
    Task<int> GenerateAndStoreOtp(string uniId);
    Task<bool> VerifyOtp(string uniId, string otpToVerify);
}