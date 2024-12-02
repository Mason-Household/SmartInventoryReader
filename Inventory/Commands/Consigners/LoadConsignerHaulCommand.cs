using MediatR;
using Inventory.Data;
using FluentValidation;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Commands.Consigners;

public class LoadConsignerHaulCommandValidator : AbstractValidator<LoadConsignerHaulCommand>
{
    public LoadConsignerHaulCommandValidator()
    {
        RuleFor(x => x.Items).NotEmpty();
        RuleFor(x => x.ConsignerId).GreaterThan(0);
    }
}

public class LoadConsignerHaulCommand : IRequest<IEnumerable<Item>>
{
    public long ConsignerId { get; set; }
    public IEnumerable<Item> Items { get; set; } = [];
}

public class LoadConsignerHaulCommandHandler(AppDbContext _context) : IRequestHandler<LoadConsignerHaulCommand, IEnumerable<Item>>
{
    public async Task<IEnumerable<Item>> Handle(LoadConsignerHaulCommand request, CancellationToken cancellationToken)
    {
        var consigner = await _context.Consigners
            .FirstOrDefaultAsync(c => c.Id == request.ConsignerId, cancellationToken) ??
            throw new KeyNotFoundException($"Consigner with ID {request.ConsignerId} not found");
        foreach (var item in request.Items)
        {
            item.ConsignerId = request.ConsignerId;
            _context.Items.Add(item);
        }

        // Update consigner's unpaid balance based on their commission rate
        decimal totalValue = request.Items.Sum(i => i.ActualPrice);
        consigner.UnpaidBalance += totalValue * consigner.CommissionRate;

        var saveResult = await _context.SaveChangesAsync(cancellationToken);
        if (saveResult <= 0) throw new InvalidOperationException("Failed to save consigner haul");
        return request.Items;
    }
}
