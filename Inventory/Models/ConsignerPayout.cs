namespace Inventory.Models;

public class ConsignerPayout : TenantEntity
{
    public new long Id { get; set; }
    public long ConsignerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PayoutDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionReference { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public Consigner Consigner { get; set; } = null!;
    public Organization Organization { get; set; } = null!;
}
