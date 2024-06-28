using ProtekTiv.Core.Interfaces.ApplicationSettings;
using System.Security.Claims;

namespace ProtekTiv.Core.Services.AuthClaims;

public class AuthClaimsService
{
    private readonly IApplicationSettings _appSettings;
    private readonly CancellationToken _cancellationToken;

    public AuthClaimsService(
        IApplicationSettings appSettings)
    {
        _appSettings = appSettings;
        _cancellationToken = new CancellationTokenSource().Token;
    }
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var claimsIdentity = (ClaimsIdentity)principal.Identity;

        if (claimsIdentity == null || !claimsIdentity.IsAuthenticated) return null;

        if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
        {

        }

        return Task.FromResult(principal);
    }
}
