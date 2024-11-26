using MediatR;
using FluentValidation;
using Inventory.Models;
using Inventory.Services;
using Inventory.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Queries.Organizations;

public class GetOrganizationsQuery : IRequest<List<Organization>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool SortAscending { get; set; } = true;
}

public class GetOrganizationsQueryHandler(
    AppDbContext _context,
    ICurrentUserService _currentUserService
) : IRequestHandler<GetOrganizationsQuery, List<Organization>>
{
    public async Task<List<Organization>> Handle(
        GetOrganizationsQuery request,
        CancellationToken cancellationToken
    ) =>
        await GetOrganizations(request, cancellationToken);

    private async Task<List<Organization>> GetOrganizations(
        GetOrganizationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId is null)
            throw new UnauthorizedAccessException("No valid user ID found in the current context");

        var query = _context.Organizations
            .Include(o => o.Users)
            .Where(o => o.Users.Any(u => u.Id == userId) && o.IsActive);

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(o => o.Name.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrEmpty(request.SortBy))
        {
            var property = typeof(Organization).GetProperty(request.SortBy);
            if (property != null)
            {
                query = request.SortAscending
                    ? query.OrderBy(o => EF.Property<object>(o, request.SortBy))
                    : query.OrderByDescending(o => EF.Property<object>(o, request.SortBy));
            }
        }

        return await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
    }
}
