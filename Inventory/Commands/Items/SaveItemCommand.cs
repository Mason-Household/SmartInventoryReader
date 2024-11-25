using MediatR;
using FluentValidation;
using Inventory.Models;
using Inventory.Services;
using Inventory.Repositories;

namespace Inventory.Commands.Items;

public class SaveItemCommandValidator : AbstractValidator<SaveItemCommand>
{
    public SaveItemCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.ActualPrice).GreaterThan(0);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.LowStockThreshold).GreaterThanOrEqualTo(0);
    }
}

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
        var organizationId = _currentUserService.GetCurrentOrganizationId() ?? throw new UnauthorizedAccessException("User is not associated with any organization");
        var organization = await _organizationRepository.GetAsync(o => o.Id == organizationId, cancellationToken);
        if (!organization.Any())
            throw new InvalidOperationException("Organization not found");

        var org = organization[0];
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
        foreach (var tagName in request.TagNames)
        {
            Tag? tag = null;
            var existingTags = await _tagRepository.GetAsync(t => t.Name == tagName);
            if (!existingTags.Any())
            {
                tag = new Tag { Name = tagName, Organization = org };
                tag = await _tagRepository.AddAsync(tag, cancellationToken);
            }
            else
            {
                tag = existingTags[0];
            }

            item.Tags.Add(tag);
        }
        return await _itemRepository.AddAsync(item, cancellationToken);
    }
}
