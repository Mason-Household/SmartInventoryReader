using MediatR;
using Inventory.Models;
using Inventory.Queries;
using Microsoft.AspNetCore.Mvc;
using Inventory.Commands.Consigners;

namespace Inventory.Controllers;

public class ConsignersController(IMediator _mediator) : BaseSmartInventoryController(_mediator)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Consigner>>> GetConsigners([FromQuery] bool includeInactive = false)
    {
        var consigners = await _mediator.Send(new GetConsignersQuery { IncludeInactive = includeInactive });
        return Ok(consigners);
    }

    [HttpGet($"{{id}}")]
    public async Task<ActionResult<Consigner>> GetConsigner([FromQuery] long id)
    {
        var consigner = await _mediator.Send(new GetConsignersQuery { Id = id });

        if (consigner == null) return NotFound();
        
        return Ok(consigner);
    }

    [HttpPut]
    public async Task<ActionResult<Consigner>> UpsertConsigner([FromBody] UpsertConsignerCommand request)
    {
        var result = await _mediator.Send(request);
        return CreatedAtAction(nameof(GetConsigner), new { id = result.Id }, result);
    }

    [HttpPut($"{{id}}")]
    public async Task<ActionResult<Consigner>> UpdateConsigner([FromQuery] long id, UpsertConsignerCommand request)
    {
        request.Id = id;
        return Ok(await _mediator.Send(request));
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
