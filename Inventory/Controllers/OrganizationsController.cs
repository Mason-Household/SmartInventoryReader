using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Inventory.Queries;
using Inventory.Commands.Organizations;

namespace Inventory.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationsController(IMediator mediator) : ControllerBase
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

        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrganization(
        CreateOrganizationCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetOrganizations), new { id = result.Id }, result);
    }
}
