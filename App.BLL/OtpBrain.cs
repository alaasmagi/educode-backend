using OtpNet;
using System;
using System.Security.Cryptography;
using System.Text;

namespace App.BLL;

public class OtpBrain
{
    private static string GenerateDynamicSecret(string uniId)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(uniId));
        var base32Secret = Base32Encoding.ToString(hash);
        
        return base32Secret;
    }

    public string GenerateTOTP(string uniId)
    {
        var dynamicSecret = GenerateDynamicSecret(uniId);
        var secretBytes = Base32Encoding.ToBytes(dynamicSecret);
        var totp = new Totp(secretBytes, step: 300);
        var otp = totp.ComputeTotp();

        return otp;
    }

    public bool VerifyTOTP(string uniId, string otpToVerify)
    {
        var dynamicSecret = GenerateDynamicSecret(uniId);
        var secretBytes = Base32Encoding.ToBytes(dynamicSecret);
        var totp = new Totp(secretBytes, step: 300);
        var isValid = totp.VerifyTotp(otpToVerify, out var timeStepMatched);

        return isValid;
    }
}