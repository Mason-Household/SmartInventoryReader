using MediatR;
using Inventory.Properties;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.Controllers;

[ApiController]
[ExcludeFromCodeCoverage]
[Route(ConfigurationConstants.InventoryApiRoute)]
public class BaseSmartInventoryController(IMediator mediator) : ControllerBase
{
    protected readonly IMediator _mediator = mediator;
}