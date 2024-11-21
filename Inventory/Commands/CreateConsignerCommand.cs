
using Inventory.Models;
using MediatR;

namespace Inventory.Commands;

public class CreateConsignerCommand : IRequest<Consigner>
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string PaymentDetails { get; set; } = null!;
    public decimal CommissionRate { get; set; }
    public string Notes { get; set; } = null!;
    public bool IsActive { get; set; }
}
