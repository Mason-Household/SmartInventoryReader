using MediatR;
using Inventory.Models;
using Inventory.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Inventory.Commands.Organizations;

namespace Inventory.Controllers;

[Authorize]
public class OrganizationsController(IMediator _mediator) : BaseSmartInventoryController(_mediator)
{
    [HttpGet]
    [Route(nameof(GetOrganizations))]
    [ProducesResponseType(typeof(IEnumerable<Organization>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOrganizations(CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetOrganizationsQuery(), cancellationToken));
    }

    [HttpPost]
    [Route(nameof(CreateOrganization))]
    [ProducesResponseType(typeof(Organization), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateOrganization(
        CreateOrganizationCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetOrganizations), new { id = result.Id }, result);
    }
}
