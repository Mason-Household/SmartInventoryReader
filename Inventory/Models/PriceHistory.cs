namespace Inventory.Models;

public class PriceHistory : AuditableEntity
{
    public new long Id { get; set; }
    public long ItemId { get; set; }
    public decimal Price { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string? Notes { get; set; }
    
    public Item Item { get; set; } = null!;
}
