namespace Inventory.Models;

public abstract class TenantEntity
{
    public long Id { get; set; }
    public long OrganizationId { get; set; }
}