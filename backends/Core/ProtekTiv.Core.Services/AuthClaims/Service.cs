using ProtekTiv.Core.Interfaces.ApplicationSettings;
using System.Security.Claims;

namespace ProtekTiv.Core.Services.AuthClaims;

public class Service(
    IApplicationSettings appSettings)
{
    private readonly IApplicationSettings _appSettings = appSettings;
    private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal == null || principal.Identity == null)
        {
            return Task.FromResult(new ClaimsPrincipal());
        } else
        {
            if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
            {
                return Task.FromResult(principal);
            }

        }
        
        return Task.FromResult(new ClaimsPrincipal());
    }
}
