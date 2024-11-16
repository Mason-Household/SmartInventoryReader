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
        return userId != null ? Guid.Parse(userId) : null;
    }

    public long? GetCurrentOrganizationId()
    {
        var organizationId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("OrganizationId");
        return organizationId != null ? long.Parse(organizationId) : null;
    }

    public string? GetUserEmail()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    }
}
