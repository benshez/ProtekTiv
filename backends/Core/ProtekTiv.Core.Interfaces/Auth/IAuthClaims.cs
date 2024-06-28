using System.Security.Claims;

namespace ProtekTiv.Core.Interfaces.Auth;

public interface IAuthClaims
{
    T QueryService<T>(T service);
    Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal);
}
