using MediatR;
using Inventory.Data;
using Inventory.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Commands.Consigners;

public class UpsertConsignerCommandValidator : AbstractValidator<UpsertConsignerCommand>
{
    public UpsertConsignerCommandValidator()
    {
        RuleFor(q => q.Name).NotEmpty();
        RuleFor(q => q.Email).NotEmpty();
        RuleFor(q => q.Phone).NotEmpty();
        RuleFor(q => q.PaymentDetails).NotEmpty();
        RuleFor(q => q.CommissionRate).GreaterThanOrEqualTo(0);
    }
}

public class UpsertConsignerCommand : IRequest<Consigner>
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string PaymentDetails { get; set; } = null!;
    public decimal CommissionRate { get; set; }
    public string Notes { get; set; } = null!;
    public bool IsActive { get; set; }
}

public class UpsertConsignerCommandHandler(AppDbContext _context) : IRequestHandler<UpsertConsignerCommand, Consigner>
{
    public async Task<Consigner> Handle(
        UpsertConsignerCommand request,
        CancellationToken cancellationToken = default
    ) =>
        await UpsertConsigner(request, cancellationToken);

    private async Task<Consigner> UpsertConsigner(
        UpsertConsignerCommand request,
        CancellationToken cancellationToken
    )
    {
        Consigner? toReturn = null;
        if (request.Id == 0)
        {
            toReturn = new Consigner
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                PaymentDetails = request.PaymentDetails,
                CommissionRate = request.CommissionRate,
                Notes = request.Notes,
                IsActive = request.IsActive
            };
            _context.Consigners.Add(toReturn);
        }
        else
        {
            toReturn = await _context.Consigners
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken) ??
                throw new KeyNotFoundException($"Consigner with ID {request.Id} not found");

            toReturn.Name = request.Name;
            toReturn.Email = request.Email;
            toReturn.Phone = request.Phone;
            toReturn.PaymentDetails = request.PaymentDetails;
            toReturn.CommissionRate = request.CommissionRate;
            toReturn.Notes = request.Notes;
            toReturn.IsActive = request.IsActive;
        }

        var saveResult = await _context.SaveChangesAsync(cancellationToken: cancellationToken);
        if (saveResult <= 0) throw new Exception("Error saving consigner");

        return toReturn;
    }
}
