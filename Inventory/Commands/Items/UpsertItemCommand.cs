using MediatR;
using Inventory.Data;
using Inventory.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Commands.Items;

public class UpsertItemCommandValidator : AbstractValidator<UpsertItemCommand>
{
    public UpsertItemCommandValidator()
    {
        RuleFor(q => q.Name).NotEmpty();
        RuleFor(q => q.CategoryId).GreaterThan(0);
        RuleFor(q => q.SuggestedPrice).GreaterThanOrEqualTo(0);
        RuleFor(q => q.ActualPrice).GreaterThanOrEqualTo(0);
        RuleFor(q => q.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(q => q.LowStockThreshold).GreaterThanOrEqualTo(0);
    }
}

public class UpsertItemCommand : IRequest<Item>
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long CategoryId { get; set; }
    public decimal SuggestedPrice { get; set; }
    public decimal ActualPrice { get; set; }
    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; }
    public string Barcode { get; set; } = null!;
    public string Notes { get; set; } = null!;
    public bool IsActive { get; set; }
}

public class UpsertItemCommandHandler(AppDbContext _context) : IRequestHandler<UpsertItemCommand, Item>
{
    public async Task<Item> Handle(
        UpsertItemCommand request,
        CancellationToken cancellationToken = default
    ) =>
        await UpsertItem(request, cancellationToken);

    private async Task<Item> UpsertItem(
        UpsertItemCommand request,
        CancellationToken cancellationToken
    )
    {
        Item? toReturn = null;
        if (request.Id == 0)
        {
            toReturn = new Item
            {
                Name = request.Name,
                CategoryId = request.CategoryId,
                SuggestedPrice = request.SuggestedPrice,
                ActualPrice = request.ActualPrice,
                StockQuantity = request.StockQuantity,
                LowStockThreshold = request.LowStockThreshold,
                Barcode = request.Barcode,
                Notes = request.Notes,
                IsDeleted = request.IsActive
            };
            _context.Items.Add(toReturn);
        }
        else
        {
            toReturn = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken) ??
                throw new KeyNotFoundException($"Item with ID {request.Id} not found");

            toReturn.Name = request.Name;
            toReturn.CategoryId = request.CategoryId;
            toReturn.SuggestedPrice = request.SuggestedPrice;
            toReturn.ActualPrice = request.ActualPrice;
            toReturn.StockQuantity = request.StockQuantity;
            toReturn.LowStockThreshold = request.LowStockThreshold;
            toReturn.Barcode = request.Barcode;
            toReturn.Notes = request.Notes;
            toReturn.IsDeleted = request.IsActive;
        }

        var saveResult = await _context.SaveChangesAsync(cancellationToken: cancellationToken);
        if (saveResult <= 0) throw new Exception("Error saving item");

        return toReturn;
    }
}