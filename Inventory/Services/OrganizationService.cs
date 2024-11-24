using Microsoft.EntityFrameworkCore;
using Inventory.Data;
using Inventory.Models;

namespace Inventory.Services;

public interface IOrganizationService
{
    Task<Organization> GetOrCreateOrganizationForUserAsync(ApplicationUser user, string authProvider, string? externalUserId);
    Task<Organization?> GetOrganizationByDomainAsync(string domain);
    Task<bool> IsUserInOrganizationAsync(Guid userId, long organizationId);
}

public class OrganizationService : IOrganizationService
{
    private readonly AppDbContext _context;

    public OrganizationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Organization> GetOrCreateOrganizationForUserAsync(
        ApplicationUser user,
        string authProvider,
        string? externalUserId)
    {
        if (string.IsNullOrEmpty(user.Email))
            throw new ArgumentException("User must have an email address");

        string domain = user.Email.Split('@')[1];
        
        // Try to find existing organization by domain
        var organization = await GetOrganizationByDomainAsync(domain);
        
        if (organization == null)
        {
            // Create new organization if none exists
            organization = new Organization
            {
                Name = $"{domain.Split('.')[0]} Organization",
                Slug = domain.Split('.')[0].ToLower(),
                Domain = domain,
                IsActive = true,
                AllowedAuthProviders = new[] { authProvider },
                AllowedEmailDomains = new[] { domain }
            };

            _context.Organizations.Add(organization);
            await _context.SaveChangesAsync();
        }
        else
        {
            // Update allowed auth providers if necessary
            if (!organization.AllowedAuthProviders.Contains(authProvider))
            {
                organization.AllowedAuthProviders = organization.AllowedAuthProviders
                    .Concat(new[] { authProvider })
                    .ToArray();
                await _context.SaveChangesAsync();
            }
        }

        // Check if user is already linked to organization
        var existingUserOrg = await _context.UserOrganizations
            .FirstOrDefaultAsync(uo => 
                uo.UserId == user.Id && 
                uo.OrganizationId == organization.Id);

        if (existingUserOrg == null)
        {
            // Determine role (first user is Owner, others are Members)
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

        return organization;
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
