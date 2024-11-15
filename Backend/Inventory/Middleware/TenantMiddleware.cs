using Inventory.Services;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext, ICurrentUserService currentUserService)
    {
        var orgId = await GetOrganizationFromRequest(context, dbContext, currentUserService);
        
        if (orgId != null)
        {
            context.Items["OrganizationId"] = orgId;
        }

        await _next(context);
    }

    private async Task<Guid?> GetOrganizationFromRequest(
        HttpContext context, 
        AppDbContext dbContext, 
        ICurrentUserService currentUserService)
    {
        // First try from subdomain
        var host = context.Request.Host.Host;
        var orgSlug = host.Split('.')[0];
        
        var org = await dbContext.Organizations
            .FirstOrDefaultAsync(o => o.Slug == orgSlug);

        if (org != null)
            return org.Id;

        // Then try from user's current organization
        var userId = currentUserService.GetCurrentUserId();
        if (userId.HasValue)
        {
            var userOrg = await dbContext.UserOrganizations
                .FirstOrDefaultAsync(uo => uo.UserId == userId);
            
            return userOrg?.OrganizationId;
        }

        return null;
    }
}