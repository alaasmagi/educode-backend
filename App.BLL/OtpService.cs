using System.Security.Cryptography;
using App.DAL.EF;
using App.Domain;
using Contracts;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace App.BLL;

public class OtpService : IOtpService
{
    private readonly ILogger<OtpService> _logger;
    private readonly RedisRepository _redisRepository;
    
    public OtpService(IConnectionMultiplexer redis, ILogger<OtpService> logger, IConnectionMultiplexer connectionMultiplexer, ILogger<RedisRepository> redisLogger)
    {
        _logger = logger;
        _redisRepository = new RedisRepository(connectionMultiplexer, redisLogger);
    }
    
    public async Task<bool> GenerateAndStoreOtp(Guid userId)
    {
        var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var otp = BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF;

        var otpExpirationMinutes = 5; // TODO: ENV!
        var status = await _redisRepository.SetDataAsync(Constants.OtpPrefix + userId, otp.ToString(), 
                                                                    TimeSpan.FromMinutes(otpExpirationMinutes));
        
        return status;
    }

    public async Task<bool> VerifyOtp(Guid userId, string otpToVerify)
    {
        var originalOtp =  await _redisRepository.GetDataAsync(Constants.OtpPrefix + userId);

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