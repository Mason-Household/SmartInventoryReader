using System.Security.Claims;

namespace Inventory.Services;

public interface ICurrentUserService
{
    string? GetUserEmail();
    Guid? GetCurrentUserId();
    long? GetCurrentOrganizationId();
}

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid? GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userId, out var result)) return result;
        return null;
    }

    public long? GetCurrentOrganizationId()
    {
        var organizationId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("OrganizationId");
        if (long.TryParse(organizationId, out var result)) return result;
        return null;
    }

    public string? GetUserEmail()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    }
}
