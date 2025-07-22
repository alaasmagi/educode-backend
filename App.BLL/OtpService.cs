using System.Security.Cryptography;
using App.DAL.EF;
using Contracts;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace App.BLL;

public class OtpService : IOtpService
{
    private readonly ILogger<OtpService> _logger;
    private readonly CacheRepository _cacheRepository;
    
    public OtpService(IConnectionMultiplexer redis, ILogger<OtpService> logger)
    {
        _logger = logger;
        _cacheRepository = new CacheRepository(redis);
    }
    
    public async Task<bool> GenerateAndStoreOtp(Guid userId)
    {
        var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var otp = BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF;

        var status = await _cacheRepository.SetOtpAsync(userId, otp.ToString());
        
        return status;
    }

    public async Task<bool> VerifyOtp(Guid userId, string otpToVerify)
    {
        var originalOtp =  await _cacheRepository.GetOtpAsync(userId);

        if (originalOtp == null)
        {
            // TODO: Error logging
            return false;
        }

        if (originalOtp == otpToVerify)
        {
            // TODO: Logging
            return true;
        }

        // TODO: Logging
        return false;
    }
}