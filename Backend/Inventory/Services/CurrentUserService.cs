namespace Inventory.Services;

public interface ICurrentUserService
{
    long? GetCurrentUserId();
    long? GetCurrentOrganizationId();
    string? GetUserEmail();
}

