using MediatR;
using Inventory.Data;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Queries.Consigners;

public class GetConsignersQuery : IRequest<IEnumerable<Consigner>>
{
    public long? Id { get; set; } = null;
    public bool IncludeInactive { get; set; }
}

public class GetConsignersQueryHandler(AppDbContext context) : IRequestHandler<GetConsignersQuery, IEnumerable<Consigner>>
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<Consigner>> Handle(GetConsignersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Consigners
            .Include(c => c.Items)
            .Include(c => c.Payouts)
            .AsNoTracking();

        if (!request.IncludeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        if (request.Id.HasValue)
        {
            query = query.Where(c => c.Id == request.Id);
        }

        return await query.ToListAsync(cancellationToken);
    }
}