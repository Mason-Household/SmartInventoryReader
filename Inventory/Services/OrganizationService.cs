using Inventory.Data;
using Inventory.Models;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin.Auth.Multitenancy;

namespace Inventory.Services;

public interface IOrganizationService
{
    Task<Organization> GetOrCreateOrganizationForUserAsync(ApplicationUser user, string authProvider, string? externalUserId);
    Task<Organization?> GetOrganizationByDomainAsync(string domain);
    Task<bool> IsUserInOrganizationAsync(Guid userId, long organizationId);
}

public class OrganizationService(AppDbContext _context) : IOrganizationService
{

    public async Task<Organization> GetOrCreateOrganizationForUserAsync(
        ApplicationUser user,
        string authProvider,
        string? externalUserId
    )
    {
        if (string.IsNullOrEmpty(user.Email))
            throw new ArgumentException("User must have an email address");

        string domain = user.Email.Split('@')[1];

        var organization = await GetOrganizationByDomainAsync(domain);

        if (organization == null)
        {
            // Create Firebase tenant
            var tenantOptions = new TenantArgs
            {
                DisplayName = $"{domain.Split('.')[0]} Organization",
                PasswordSignUpAllowed = true,
                EmailLinkSignInEnabled = true,
            };

            var tenant = await FirebaseAuth.DefaultInstance.TenantManager.CreateTenantAsync(tenantOptions);

            organization = new Organization
            {
                Name = $"{domain.Split('.')[0]} Organization",
                Slug = domain.Split('.')[0].ToLower(),
                Domain = domain,
                IsActive = true,
                AllowedAuthProviders = [authProvider],
                AllowedEmailDomains = [domain],
                FirebaseTenantId = tenant.TenantId
            };

            _context.Organizations.Add(organization);
            await _context.SaveChangesAsync();
        }

        await AddUserToOrganizationAsync(user, organization, authProvider, externalUserId);

        return organization;
    }

    private async Task AddUserToOrganizationAsync(
        ApplicationUser user, 
        Organization organization, 
        string authProvider, 
        string? externalUserId
    )
    {
        var existingUserOrg = await _context.UserOrganizations
            .FirstOrDefaultAsync(uo =>
                uo.UserId == user.Id &&
                uo.OrganizationId == organization.Id);

        if (existingUserOrg == null)
        {
            var isFirstUser = !await _context.UserOrganizations
                .AnyAsync(uo => uo.OrganizationId == organization.Id);

            var userOrganization = new UserOrganization
            {
                UserId = user.Id,
                OrganizationId = organization.Id,
                Role = isFirstUser ? "Owner" : "Member",
                JoinedAt = DateTime.UtcNow,
                AuthProvider = authProvider,
                ExternalUserId = externalUserId
            };

            _context.UserOrganizations.Add(userOrganization);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Organization?> GetOrganizationByDomainAsync(string domain)
    {
        return await _context.Organizations
            .FirstOrDefaultAsync(o =>
                o.Domain == domain ||
                o.AllowedEmailDomains.Contains(domain));
    }

    public async Task<bool> IsUserInOrganizationAsync(Guid userId, long organizationId)
    {
        return await _context.UserOrganizations
            .AnyAsync(uo =>
                uo.UserId == userId &&
                uo.OrganizationId == organizationId);
    }
}