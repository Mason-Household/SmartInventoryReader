using MediatR;
using Inventory.Data;
using FluentValidation;
using Inventory.Models;
using Inventory.Services;

namespace Inventory.Commands.Organizations;

public class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(200);
    }
}

public class CreateOrganizationCommand : IRequest<Organization>
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}

public class CreateOrganizationCommandHandler(
    AppDbContext _context,
    ICurrentUserService _currentUserService
) : IRequestHandler<CreateOrganizationCommand, Organization>
{
    public async Task<Organization> Handle(
        CreateOrganizationCommand request,
        CancellationToken cancellationToken
    ) =>
        await CreateOrganization(request, cancellationToken);

    private async Task<Organization> CreateOrganization(
        CreateOrganizationCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = _currentUserService.GetCurrentUserId() ?? 
            throw new UnauthorizedAccessException("User not authenticated");

        var organization = new Organization
        {
            Name = request.Name,
            Slug = request.Slug,
            IsActive = request.IsActive
        };

        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync(cancellationToken);

        var userOrganization = new UserOrganization
        {
            UserId = userId,
            OrganizationId = organization.Id,
            Role = "Owner",
            JoinedAt = DateTime.UtcNow
        };

        _context.UserOrganizations.Add(userOrganization);
        var saveResult = await _context.SaveChangesAsync(cancellationToken);
        if (saveResult <= 0) throw new Exception("Error saving organization");

        return organization;
    }
}
