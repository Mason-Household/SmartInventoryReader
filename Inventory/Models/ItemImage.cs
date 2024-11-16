namespace Inventory.Models;

public class ItemImage : AuditableEntity
{
    public new long Id { get; set; }
    public long ItemId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string? Caption { get; set; }
    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
    
    public Item Item { get; set; } = null!;
}
