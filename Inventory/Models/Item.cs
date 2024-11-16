namespace Inventory.Models;

public class Item : AuditableEntity
{
    public new long Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal? SuggestedPrice { get; set; }
    public decimal ActualPrice { get; set; }
    public int StockQuantity { get; set; }
    public int? LowStockThreshold { get; set; }
    public string? Barcode { get; set; }
    public long? CategoryId { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public string? Notes { get; set; }
    public Category? Category { get; set; }
    public Organization Organization { get; set; } = null!;
    public ICollection<PriceHistory> PriceHistory { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];
    public ICollection<ItemImage> Images { get; set; } = [];
    public ICollection<InventoryTransaction> Transactions { get; set; } = [];
}
