using Inventory.Data;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Commands;

public class LoadConsignerHaulCommand
{
    private readonly AppDbContext _context;

    public LoadConsignerHaulCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Item>> ExecuteAsync(long consignerId, IEnumerable<Item> items)
    {
        var consigner = await _context.Consigners
            .FirstOrDefaultAsync(c => c.Id == consignerId);

        if (consigner == null)
        {
            throw new KeyNotFoundException($"Consigner with ID {consignerId} not found");
        }

        foreach (var item in items)
        {
            item.ConsignerId = consignerId;
            item.SuggestedPrice = item.ActualPrice; // Store the original price as suggested
            _context.Items.Add(item);
        }

        // Update consigner's unpaid balance based on their commission rate
        decimal totalValue = items.Sum(i => i.ActualPrice);
        consigner.UnpaidBalance += totalValue * consigner.CommissionRate;

        await _context.SaveChangesAsync();
        return items;
    }
}
