using MediatR;
using FluentValidation;
using Inventory.Models;
using Inventory.Repositories;

namespace Inventory.Queries;

public class GetUnpaidBalanceQueryValidator : AbstractValidator<GetUnpaidBalanceQuery>
{
    public GetUnpaidBalanceQueryValidator()
    {
        RuleFor(q => q.ConsignerId).GreaterThan(0);
    }
}

public class GetUnpaidBalanceQuery : IRequest<decimal>
{
    public long ConsignerId { get; set; }
}

public class GetUnpaidBalanceQueryHandler(IRepository<Consigner> _consignerRepository) : IRequestHandler<GetUnpaidBalanceQuery, decimal>
{
    public async Task<decimal> Handle(GetUnpaidBalanceQuery request, CancellationToken cancellationToken)
    {
        var consigner = await _consignerRepository.GetByIdAsync(request.ConsignerId, cancellationToken)
            ?? throw new KeyNotFoundException($"Consigner with ID {request.ConsignerId} not found");
        return consigner.UnpaidBalance;
    }
}