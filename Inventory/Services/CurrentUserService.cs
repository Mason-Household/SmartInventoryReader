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
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("user_id") ?? 
                    _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");
        
        if (string.IsNullOrEmpty(userId)) return null;
        
        return CreateDeterministicGuid(userId);
    }

    public long? GetCurrentOrganizationId()
    {
        var organizationId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("OrganizationId");
        if (long.TryParse(organizationId, out var result)) return result;
        return null;
    }

    public string? GetUserEmail()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue("email") ??
               _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    }

    private static Guid CreateDeterministicGuid(string input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
        return new Guid(hashBytes);
    }
}
