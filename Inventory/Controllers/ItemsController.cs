using MediatR;
using Inventory.Queries;
using Microsoft.AspNetCore.Mvc;
using Inventory.Commands.Items;

namespace Inventory.Controllers;

public class ItemsController(IMediator _mediator) : BaseSmartInventoryController(_mediator)
{
    [HttpPost]
    public async Task<IActionResult> SaveItem([FromBody] SaveItemCommand command) => Ok(await _mediator.Send(command));

    [HttpGet]
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
    public async Task<IActionResult> UpsertItem([FromBody] UpsertItemCommand command) => Ok(await _mediator.Send(command));

    [HttpDelete]
    public async Task<IActionResult> DeleteItem([FromQuery] long id) => Ok(await _mediator.Send(new DeleteItemCommand(id)));
    
}
