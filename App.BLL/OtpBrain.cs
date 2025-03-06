using OtpNet;
using System;
using System.Security.Cryptography;
using System.Text;

namespace App.BLL;

public class OtpBrain
{
    private static string GenerateDynamicSecret(string userUniId)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(userUniId));
        var base32Secret = Base32Encoding.ToString(hash);
        
        return base32Secret;
    }

    public string GenerateTOTP(string userUniId)
    {
        var dynamicSecret = GenerateDynamicSecret(userUniId);
        var secretBytes = Base32Encoding.ToBytes(dynamicSecret);
        var totp = new Totp(secretBytes, step: 120);
        var otp = totp.ComputeTotp();

        return otp;
    }

    public bool VerifyTOTP(string userUniId, string otpToVerify)
    {
        var dynamicSecret = GenerateDynamicSecret(userUniId);
        var secretBytes = Base32Encoding.ToBytes(dynamicSecret);
        var totp = new Totp(secretBytes, step: 120);
        var isValid = totp.VerifyTotp(otpToVerify, out var timeStepMatched);

        return isValid;
    }
}