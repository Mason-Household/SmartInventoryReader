namespace Inventory.Models;

public enum TransactionType
{
    Purchase,
    Sale,
    Adjustment,
    Return
}

public class InventoryTransaction : AuditableEntity
{
    public new long Id { get; set; }
    public long ItemId { get; set; }
    public TransactionType Type { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public DateTime TransactionDate { get; set; }
    
    public Item Item { get; set; } = null!;
}
