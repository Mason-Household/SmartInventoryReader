using MediatR;
using Inventory.Models;
using Inventory.Queries.Organizations;
using Microsoft.AspNetCore.Mvc;
using Inventory.Commands.Organizations;

namespace Inventory.Controllers;

public class OrganizationsController(IMediator _mediator) : BaseSmartInventoryController(_mediator)
{
    [HttpGet]
    [Route(nameof(GetOrganizations))]
    [ProducesResponseType(typeof(IEnumerable<Organization>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganizations(CancellationToken cancellationToken = default)
    {
        var organizations = await _mediator.Send(new GetOrganizationsQuery(), cancellationToken);
        return Ok(organizations);
    }

    [HttpPost]
    [Route(nameof(CreateOrganization))]
    [ProducesResponseType(typeof(Organization), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateOrganization(
        CreateOrganizationCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetOrganizations), new { id = result.Id }, result);
    }
}
