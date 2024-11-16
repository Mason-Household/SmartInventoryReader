using MediatR;
using Inventory.Models;
using Inventory.Repositories;

namespace Inventory.Commands;

public class SaveItemCommand : IRequest<Item>
{
    public string Name { get; set; } = null!;
    public decimal? SuggestedPrice { get; set; }
    public decimal ActualPrice { get; set; }
    public int StockQuantity { get; set; }
    public int? LowStockThreshold { get; set; }
    public string? Barcode { get; set; }
    public Guid? CategoryId { get; set; }
    public List<string> TagNames { get; set; } = [];
    public string? Notes { get; set; }
}

public class SaveItemCommandHandler(
    IRepository<Item> _itemRepository, 
    IRepository<Tag> _tagRepository
) : IRequestHandler<SaveItemCommand, Item>
{

    public async Task<Item> Handle(
        SaveItemCommand request, 
        CancellationToken cancellationToken
    )
    {
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            SuggestedPrice = request.SuggestedPrice,
            ActualPrice = request.ActualPrice,
            StockQuantity = request.StockQuantity,
            LowStockThreshold = request.LowStockThreshold,
            Barcode = request.Barcode,
            CategoryId = request.CategoryId,
            Notes = request.Notes
        };

        foreach (var tagName in request.TagNames)
        {
            var tag = await _tagRepository.GetAsync(t => t.Name == tagName);
            var existingTag = tag.FirstOrDefault();
            if (existingTag is null)
            {
                existingTag = new Tag { Name = tagName };
                await _tagRepository.AddAsync(existingTag);
            }
            item.Tags.Add(existingTag);
        }
        return await _itemRepository.AddAsync(item);
    }
}
