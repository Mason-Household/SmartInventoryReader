using MediatR;
using Inventory.Properties;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Inventory.Controllers;

[ApiController]
[ExcludeFromCodeCoverage]
[Route(ConfigurationConstants.InventoryApiRoute)]
public class BaseSmartInventoryController(IMediator mediator) : ControllerBase
{
    protected readonly IMediator _mediator = mediator;
}