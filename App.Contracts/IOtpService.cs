namespace Contracts;

public interface IOtpService
{
    string GenerateTotp(string uniId);
    bool VerifyTotp(string uniId, string otpToVerify);
}