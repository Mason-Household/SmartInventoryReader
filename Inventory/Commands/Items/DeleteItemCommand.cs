using MediatR;
using FluentValidation;
using Inventory.Models;
using Inventory.Repositories;

namespace Inventory.Commands.Items;

public class DeleteItemCommandValidator : AbstractValidator<DeleteItemCommand>
{
    public DeleteItemCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public record DeleteItemCommand(long Id) : IRequest<bool>;

public class DeleteItemCommandHandler(
    IRepository<Item> _itemRepository,
    ILogger<DeleteItemCommandHandler> _logger
) : IRequestHandler<DeleteItemCommand, bool>
{
    public async Task<bool> Handle(
        DeleteItemCommand request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var item = await _itemRepository.GetByIdAsync(request.Id);

            if (item == null) return false;

            item.IsDeleted = true;

            await _itemRepository.UpdateAsync(item);
            _logger.LogInformation("Item with ID {Id} soft deleted", request.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting item with ID {Id}", request.Id);
            return false;
        }

    }
}