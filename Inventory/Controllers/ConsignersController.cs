using Inventory.Commands;
using Inventory.Models;
using Inventory.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<Consigner>> GetConsigner(long id)
    {
        var consigner = await _mediator.Send(new GetConsignersQuery { Id = id });

        if (consigner == null) return NotFound();
        
        return Ok(consigner);
    }

    [HttpPost]
    public async Task<ActionResult<Consigner>> CreateConsigner([FromBody] CreateConsignerCommand consigner)
    {
        if (consigner.Id != 0)
        {
            return BadRequest("ID must not be specified when creating a new consigner");
        }

        var result = await _mediator.Send(consigner);
        return CreatedAtAction(nameof(GetConsigner), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Consigner>> UpdateConsigner(long id, Consigner consigner)
    {
        if (id != consigner.Id)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            var result = await _mediator.Send(consigner);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/unpaid-balance")]
    public async Task<ActionResult<decimal>> GetUnpaidBalance(long id)
    {
        try
        {
            var balance = await _mediator.Send(new GetUnpaidBalanceQuery { ConsignerId = id });
            return Ok(balance);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/payouts")]
    public async Task<ActionResult<ConsignerPayout>> RecordPayout(long id, ConsignerPayout payout)
    {
        if (id != payout.ConsignerId)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            var result = await _mediator.Send(payout);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
