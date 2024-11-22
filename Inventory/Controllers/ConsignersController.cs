using MediatR;
using Inventory.Models;
using Inventory.Queries;
using Inventory.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ConsignersController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Consigner>>> GetConsigners([FromQuery] bool includeInactive = false)
    {
        var consigners = await _mediator.Send(new GetConsignersQuery { IncludeInactive = includeInactive });
        return Ok(consigners);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Consigner>> GetConsigner([FromQuery] long id)
    {
        var consigner = await _mediator.Send(new GetConsignersQuery { Id = id });

        if (consigner == null) return NotFound();
        
        return Ok(consigner);
    }

    [HttpPost]
    public async Task<ActionResult<Consigner>> CreateConsigner([FromBody] UpsertConsignerCommand request)
    {
        var result = await _mediator.Send(request);
        return CreatedAtAction(nameof(GetConsigner), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Consigner>> UpdateConsigner([FromQuery] long id, UpsertConsignerCommand request)
    {
        try
        {
            request.Id = id;
            return Ok(await _mediator.Send(request));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet($"{{id}}/unpaid-balance")]
    public async Task<ActionResult<decimal>> GetUnpaidBalance([FromQuery] long id)
    {
        var balance = await _mediator.Send(new GetUnpaidBalanceQuery { ConsignerId = id });
        return Ok(balance);
    }

    [HttpPost($"{{id}}/payouts")]
    public async Task<ActionResult<ConsignerPayout>> RecordPayout(
        [FromQuery] long id, 
        [FromBody] RecordConsignerPayoutCommand request
    )
    {
        request.ConsignerId = id;
        return Ok(await _mediator.Send(request));
    }
}
