namespace Inventory.Models;

public class Consigner : TenantEntity
{
    public new long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? PaymentDetails { get; set; }
    public decimal UnpaidBalance { get; set; }
    public decimal TotalPaidOut { get; set; }
    public decimal CommissionRate { get; set; } = 0.7M; // Default 70% to consigner
    public string? Notes { get; set; }
    
    // Navigation properties
    public Organization Organization { get; set; } = null!;
    public ICollection<Item> Items { get; set; } = [];
    public ICollection<ConsignerPayout> Payouts { get; set; } = [];
    public bool IsActive { get; set; } = true;
}
