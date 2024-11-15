namespace Inventory.Models;

public class Organization
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;  // URL-friendly name
    public string? LogoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public string? SubscriptionTier { get; set; }
    // Navigation properties
    public ICollection<ApplicationUser> Users { get; set; } = [];
    // public ICollection<OrganizationInvite> Invites { get; set; } = [];
}