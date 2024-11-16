using Inventory.Application.Commands;
using Inventory.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inventory.Controllers;

[ApiController]
[Route(ConfigurationConstants.ItemsRoute)]
[ApiVersion(ConfigurationConstants.ApiVersion)]
public class ItemsController(IMediator _mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveItem([FromBody] SaveItemCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
