using MediatR;
using Inventory.Properties;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.Controllers;

[Authorize]
[ApiController]
[Route(ConfigurationConstants.InventoryApiRoute)]
[ExcludeFromCodeCoverage]
public class BaseSmartInventoryController(IMediator mediator) : ControllerBase
{
    protected readonly IMediator _mediator = mediator;
}