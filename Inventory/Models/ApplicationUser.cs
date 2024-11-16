using Microsoft.AspNetCore.Identity;

namespace Inventory.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public long? CurrentOrganizationId { get; set; }
    
    public ICollection<UserOrganization> Organizations { get; set; } = [];
}