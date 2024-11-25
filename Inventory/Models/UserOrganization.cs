using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Models;

[PrimaryKey(nameof(UserId), nameof(OrganizationId))]
public class UserOrganization
{
    public Guid UserId { get; set; }
    public long OrganizationId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = null!; // Admin, Member, etc.

    public DateTime JoinedAt { get; set; }

    [Required]
    [MaxLength(50)]
    public string AuthProvider { get; set; } = null!; // google, huggingface, email

    [MaxLength(200)]
    public string? ExternalUserId { get; set; } // ID from external auth provider
    
    public ApplicationUser User { get; set; } = null!;
    public Organization Organization { get; set; } = null!;
}
