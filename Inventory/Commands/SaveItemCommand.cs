using MediatR;
using Inventory.Models;
using Inventory.Repositories;
using Inventory.Services;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Commands;

public class SaveItemCommand : IRequest<Item>
{
    public string Name { get; set; } = null!;
    public decimal? SuggestedPrice { get; set; }
    public decimal ActualPrice { get; set; }
    public int StockQuantity { get; set; }
    public int? LowStockThreshold { get; set; }
    public string? Barcode { get; set; }
    public long? CategoryId { get; set; }
    public List<string> TagNames { get; set; } = [];
    public string? Notes { get; set; }
}

public class SaveItemCommandHandler(
    IRepository<Item> _itemRepository,
    IRepository<Tag> _tagRepository,
    IRepository<Organization> _organizationRepository,
    ICurrentUserService _currentUserService
) : IRequestHandler<SaveItemCommand, Item>
{
    public async Task<Item> Handle(
        SaveItemCommand request,
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

        var item = new Item
        {
            Name = request.Name,
            SuggestedPrice = request.SuggestedPrice,
            ActualPrice = request.ActualPrice,
            StockQuantity = request.StockQuantity,
            LowStockThreshold = request.LowStockThreshold,
            Barcode = request.Barcode,
            CategoryId = request.CategoryId,
            Notes = request.Notes,
            Organization = org
        };

        // Handle tags
        foreach (var tagName in request.TagNames)
        {
            var existingTags = await _tagRepository.GetAsync(t => t.Name == tagName);
            var tag = existingTags.FirstOrDefault();
            
            if (tag == null)
            {
                tag = new Tag { Name = tagName, Organization = org };
                tag = await _tagRepository.AddAsync(tag);
            }
            
            item.Tags.Add(tag);
        }

        // Save the item with its relationships
        var savedItem = await _itemRepository.AddAsync(item);
        
        return savedItem;
    }
}
