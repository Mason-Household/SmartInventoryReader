using MediatR;
using Inventory.Properties;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Inventory.Application.Commands;
using Inventory.Queries;

namespace Inventory.Controllers;

[ApiController]
[Route(ConfigurationConstants.ItemsRoute)]
[ApiVersion(ConfigurationConstants.ApiVersion)]
public class ItemsController(IMediator _mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveItem([FromBody] SaveItemCommand command) 
        => Ok(await _mediator.Send(command));

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
}
