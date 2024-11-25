namespace Inventory.Models;

public class Organization : AuditableEntity
{
    public new long OrganizationId { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;  // URL-friendly name
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public string? SubscriptionTier { get; set; }
    public string Domain { get; set; } = null!;
    public string[] AllowedAuthProviders { get; set; } = [];
    public string[] AllowedEmailDomains { get; set; } = [];
    
    // Navigation properties
    public ICollection<ApplicationUser> Users { get; set; } = [];
    public string FirebaseTenantId { get; set; } = null!;

}
