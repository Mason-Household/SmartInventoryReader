using MediatR;
using Inventory.Models;
using Inventory.Queries;
using Microsoft.AspNetCore.Mvc;
using Inventory.Commands.Items;

namespace Inventory.Controllers;

public class ItemsController(IMediator _mediator) : BaseSmartInventoryController(_mediator)
{
    [HttpPost]
    [Route(nameof(SaveItem))]
    [ProducesResponseType(typeof(Item), StatusCodes.Status201Created)]
    public async Task<IActionResult> SaveItem([FromBody] SaveItemCommand command) => Ok(await _mediator.Send(command));

    [HttpGet]
    [Route(nameof(GetItems))]
    [ProducesResponseType(typeof(IEnumerable<Item>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetItems(
        [FromQuery] long organizationId, 
        [FromQuery] long userId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10
    )
    {
        var query = new GetItemsQuery
        {
            OrganizationId = organizationId,
            UserId = userId,
            Page = page,
            PageSize = pageSize
        };
        return Ok(await _mediator.Send(query));
    }

    [HttpPut]
    [Route(nameof(UpsertItem))]
    [ProducesResponseType(typeof(Item), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpsertItem([FromBody] UpsertItemCommand command) => Ok(await _mediator.Send(command));

    [HttpDelete]
    [Route(nameof(DeleteItem))]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteItem([FromQuery] long id) => Ok(await _mediator.Send(new DeleteItemCommand(id)));
    
}
