using MediatR;
using Inventory.Queries;
using Microsoft.AspNetCore.Mvc;
using Inventory.Commands.Organizations;

namespace Inventory.Controllers;

public class OrganizationsController(IMediator _mediator) : BaseSmartInventoryController(_mediator)
{
    [HttpGet]
    public async Task<IActionResult> GetOrganizations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortAscending = true,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetOrganizationsQuery
        {
            Page = page,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            SortBy = sortBy,
            SortAscending = sortAscending
        };
        return Ok(await _mediator.Send(query, cancellationToken));
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
