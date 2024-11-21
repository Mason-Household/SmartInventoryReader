using Inventory.Data;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Commands;

public class RecordConsignerPayoutCommand
{
    private readonly AppDbContext _context;

    public RecordConsignerPayoutCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ConsignerPayout> ExecuteAsync(ConsignerPayout payout)
    {
        var consigner = await _context.Consigners
            .FirstOrDefaultAsync(c => c.Id == payout.ConsignerId);

        if (consigner == null)
        {
            throw new KeyNotFoundException($"Consigner with ID {payout.ConsignerId} not found");
        }

        // Update consigner's payout totals
        consigner.UnpaidBalance -= payout.Amount;
        consigner.TotalPaidOut += payout.Amount;

        // Set payout date if not specified
        if (payout.PayoutDate == default)
        {
            payout.PayoutDate = DateTime.UtcNow;
        }

        _context.ConsignerPayouts.Add(payout);
        await _context.SaveChangesAsync();

        return payout;
    }

    public async Task<decimal> CalculateUnpaidBalanceAsync(long consignerId)
    {
        var consigner = await _context.Consigners
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == consignerId);

        if (consigner == null)
        {
            throw new KeyNotFoundException($"Consigner with ID {consignerId} not found");
        }

        // Calculate total sales value
        decimal totalSales = await _context.Items
            .Where(i => i.ConsignerId == consignerId)
            .SumAsync(i => i.ActualPrice);

        // Calculate consigner's share based on commission rate
        decimal consignerShare = totalSales * consigner.CommissionRate;

        // Subtract already paid amount
        decimal alreadyPaid = await _context.ConsignerPayouts
            .Where(p => p.ConsignerId == consignerId)
            .SumAsync(p => p.Amount);

        return consignerShare - alreadyPaid;
    }
}
