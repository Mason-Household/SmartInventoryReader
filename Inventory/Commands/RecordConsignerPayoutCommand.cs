using MediatR;
using Inventory.Data;
using FluentValidation;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Commands;

public class RecordConsignerPayoutCommandValidator : AbstractValidator<RecordConsignerPayoutCommand>
{
    public RecordConsignerPayoutCommandValidator()
    {
        RuleFor(x => x.ConsignerId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.PaymentMethod).NotEmpty();
    }
}

public class RecordConsignerPayoutCommand : IRequest<Models.ConsignerPayout>
{
    public long ConsignerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PayoutDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}

public class RecordConsignerPayoutCommandHandler(AppDbContext _context) : IRequestHandler<RecordConsignerPayoutCommand, Models.ConsignerPayout>
{
    public async Task<Models.ConsignerPayout> Handle(
        RecordConsignerPayoutCommand request, 
        CancellationToken cancellationToken
    )
    {
        var consigner = await _context.Consigners
            .FirstOrDefaultAsync(c => c.Id == request.ConsignerId, cancellationToken) ?? 
            throw new KeyNotFoundException($"Consigner with ID {request.ConsignerId} not found");

        // Update consigner's payout totals
        consigner.UnpaidBalance -= request.Amount;
        consigner.TotalPaidOut += request.Amount;

        // Set payout date if not specified
        if (request.PayoutDate == default)
        {
            request.PayoutDate = DateTime.UtcNow;
        }

        var payout = new Models.ConsignerPayout
        {
            ConsignerId = request.ConsignerId,
            Amount = request.Amount,
            PayoutDate = request.PayoutDate,
            PaymentMethod = request.PaymentMethod
        };

        _context.ConsignerPayouts.Add(payout);
        var saveResult = await _context.SaveChangesAsync(cancellationToken);
        if (saveResult <= 0) throw new InvalidOperationException("Failed to save consigner payout");

        return payout;
    }
}
