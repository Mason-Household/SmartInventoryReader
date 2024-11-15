namespace Inventory.Models;

public class UserOrganization
{
    public long UserId { get; set; }
    public long OrganizationId { get; set; }
    public string Role { get; set; } = null!; // Admin, Member, etc.
    public DateTime JoinedAt { get; set; }
    
    public ApplicationUser User { get; set; } = null!;
    public Organization Organization { get; set; } = null!;
}