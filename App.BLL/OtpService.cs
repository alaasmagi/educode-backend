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
    
    public async Task<int> GenerateAndStoreOtp(string uniId)
    {
        var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var otp = BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF;

        var otpExpirationMinutes = 5; // TODO: ENV!
        var status = await _redisRepository.SetDataAsync(Constants.OtpPrefix + uniId, otp.ToString(), 
                                                                    TimeSpan.FromMinutes(otpExpirationMinutes));
        if (status == false)
        {
            _logger.LogWarning($"Otp generation failed for user with id {uniId}");
        }
        
        _logger.LogInformation($"Otp generation successful for user with id {uniId}");
        return otp;
    }

    public async Task<bool> VerifyOtp(string uniId, string otpToVerify)
    {
        var originalOtp =  await _redisRepository.GetDataAsync(Constants.OtpPrefix + uniId);

        if (originalOtp == null)
        {
            _logger.LogWarning($"Otp verification failed for user with id {uniId}");
            return false;
        }

        if (originalOtp == otpToVerify)
        {
            _logger.LogWarning($"Otp verification successful for user with id {uniId}");
            return true;
        }

        _logger.LogWarning($"Otp verification failed for user with id {uniId}");
        return false;
    }
}