namespace ProtekTiv.Core.Interfaces.Auth;

public interface IAuthTokenManager
{
    Task<string> GetAuthToken(string tokenCachekey, string audience);
}
