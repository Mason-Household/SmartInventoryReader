using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Inventory.Services;

public interface ICurrentUserService
{
    Guid? GetCurrentUserId();
    long? GetCurrentOrganizationId();
    string? GetUserEmail();
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

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
