using FluentValidation;
using MediatR;
using Inventory.Models;
using Inventory.Repositories;
using Inventory.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Inventory.Queries;

public class GetItemsQueryValidator : AbstractValidator<GetItemsQuery>
{
    public GetItemsQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);
    }
}

public class GetItemsQuery : IRequest<List<Item>>
{
    public long OrganizationId { get; set; }
    public long UserId { get; set; }
    public long? CategoryId { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public bool SortAscending { get; set; }
}

public class GetItemsQueryHandler(
    IRepository<Item> _itemRepository,
    IRepository<Organization> _organizationRepository,
    ICurrentUserService _currentUserService
) : IRequestHandler<GetItemsQuery, List<Item>>
{
    public async Task<List<Item>> Handle(
        GetItemsQuery request,
        CancellationToken cancellationToken
    )
    {
        var organizationId = _currentUserService.GetCurrentOrganizationId();
        if (organizationId == null)
        {
            throw new UnauthorizedAccessException("User is not associated with any organization");
        }

        var organization = await _organizationRepository.GetAsync(o => o.Id == organizationId);
        var org = organization.FirstOrDefault() ?? 
            throw new InvalidOperationException("Organization not found");

        var items = await _itemRepository.GetAsync(
            i => i.Organization.Id == organizationId &&
                 (request.CategoryId == null || i.CategoryId == request.CategoryId) &&
                 (request.SearchTerm == null || i.Name.Contains(request.SearchTerm))
        );

        // Apply sorting if specified
        var query = items.AsQueryable();
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            var property = typeof(Item).GetProperty(request.SortBy);
            if (property != null)
            {
                query = request.SortAscending
                    ? query.OrderBy(i => property.GetValue(i))
                    : query.OrderByDescending(i => property.GetValue(i));
            }
        }

        // Apply pagination
        return query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();
    }
}
