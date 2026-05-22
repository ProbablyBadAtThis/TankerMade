using System.Security.Claims;

namespace TankerMade.Server.Controllers;

internal static class ControllerUserExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId) ? userId : null;
    }
}
