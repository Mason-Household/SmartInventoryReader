namespace Inventory.Services;

public interface ICurrentUserService
{
    long? GetCurrentUserId();
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

    public long? GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId != null ? long.Parse(userId) : (long?)null;
    }

    public long? GetCurrentOrganizationId()
    {
        var organizationId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("OrganizationId");
        return organizationId != null ? long.Parse(organizationId) : (long?)null;
    }

    public string? GetUserEmail()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    }
}

