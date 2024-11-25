using MediatR;
using Inventory.Queries;
using Microsoft.AspNetCore.Mvc;
using Inventory.Commands.Organizations;

namespace Inventory.Controllers;

public class OrganizationsController(IMediator _mediator) : BaseSmartInventoryController(_mediator)
{
    [HttpGet]
    public async Task<IActionResult> GetOrganizations(CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetOrganizationsQuery(), cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrganization(
        CreateOrganizationCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetOrganizations), new { id = result.Id }, result);
    }
}
