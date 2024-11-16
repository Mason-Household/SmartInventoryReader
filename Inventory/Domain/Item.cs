namespace Inventory.Domain;

public class InventoryItem
{
    public string? Id { get; set; }
    public required string Name { get; set; }
    public decimal? SuggestedPrice { get; set; }
    public required decimal ActualPrice { get; set; }
    InventoryItemType Type { get; set; }
    public string? Category { get; set; }
    double Confidence { get; set; }
    public required string DateAdded { get; set; }
    InventoryItemSource Source { get; set; }
    public string? Notes { get; set; }
    public string[]? Tags { get; set; }
}

public enum InventoryItemType
{
    Product = "Product",
    MenuItem = "MenuItem"
}

public enum InventoryItemSource
{
    Scan = "Scan",
    Manual = "Manual"
}