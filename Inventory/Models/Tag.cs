namespace Inventory.Models;

public class Tag : TenantEntity
{
    public new long Id { get; set; }
    public string Name { get; set; } = null!;
    public Organization Organization { get; set; } = null!;
    public ICollection<Item> Items { get; set; } = [];
}
