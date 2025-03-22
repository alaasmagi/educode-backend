namespace Contracts;

public interface IAdminAccessService
{
    bool AdminAccessGrant(string enteredUsername, string enteredPassword);
    string GenerateAdminAccessToken();
    Task<string> GetHashedAdminAccessTokenAsync(string input);
    Task<bool> CompareHashedTokensAsync(string inputToken, string hashedInputToken);
}