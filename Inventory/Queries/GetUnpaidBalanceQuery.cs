using MediatR;
using FluentValidation;

namespace Inventory.Queries;

public class GetUnpaidBalanceQueryValidator : AbstractValidator<GetUnpaidBalanceQuery>
{
    public GetUnpaidBalanceQueryValidator()
    {
        RuleFor(q => q.ConsignerId).GreaterThan(0);
    }
}

public class GetUnpaidBalanceQuery : IRequest<object>
{
    public long ConsignerId { get; set; }
}

public class GetUnpaidBalanceQueryHandler : IRequestHandler<GetUnpaidBalanceQuery, object>
{
    public Task<object> Handle(GetUnpaidBalanceQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}