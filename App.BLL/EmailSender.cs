using System;
using RestSharp; 
using RestSharp.Authenticators;
using System.Threading;
using System.Threading.Tasks;
using App.Domain;

namespace App.BLL;

public class EmailSender
{
    public static async Task<RestResponse> SendEmail(UserEntity user, string oneTimeKey)
    {
        var options = new RestClientOptions("https://api.mailgun.net/v3")
        {
            Authenticator = new HttpBasicAuthenticator("api", Environment.GetEnvironmentVariable("MAILGUN_API") ?? "API_KEY")
        };
        
        var client = new RestClient(options);
        var request = new RestRequest($"{Environment.GetEnvironmentVariable("MAILGUN_RESOURCE")}", Method.Post);
        request.AlwaysMultipartFormData = true;
        request.AddParameter("from", $"EduCode <{Environment.GetEnvironmentVariable("MAILGUN_SENDER")}>");
        request.AddParameter("to", $"{user.FullName} <{user.UniId}@taltech.ee>");
        request.AddParameter("subject", $"One time key for {user.UniId} EduCode account");
        request.AddParameter("text", "Here is your onetime key to verify ownership of Your EduCode account: \n" +
                                                                                                    $"\t{oneTimeKey}");
        return await client.ExecuteAsync(request);
    }
}