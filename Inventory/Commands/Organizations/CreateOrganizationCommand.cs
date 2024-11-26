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
    public string Domain { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public string[] AllowedAuthProviders { get; set; } = [];
    public string[] AllowedEmailDomains { get; set; } = [];
}

public class CreateOrganizationCommandHandler(
    AppDbContext context,
    ICurrentUserService currentUserService,
    IOrganizationService organizationService
) : IRequestHandler<CreateOrganizationCommand, Organization>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IOrganizationService _organizationService = organizationService;

    public async Task<Organization> Handle(
        CreateOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        // Check if organization already exists for domain
        var existingOrg = await _organizationService.GetOrganizationByDomainAsync(request.Domain);
        if (existingOrg != null)
        {
            throw new InvalidOperationException($"Organization already exists for domain {request.Domain}");
        }

        var userId = _currentUserService.GetCurrentUserId() ?? 
            throw new UnauthorizedAccessException("User not authenticated");

        var organization = new Organization
        {
            Name = request.Name,
            Slug = request.Slug,
            Domain = request.Domain,
            IsActive = request.IsActive,
            AllowedAuthProviders = request.AllowedAuthProviders,
            AllowedEmailDomains = request.AllowedEmailDomains
        };

        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync(cancellationToken);

        var userOrganization = new UserOrganization
        {
            UserId = userId,
            OrganizationId = organization.Id,
            Role = "Owner",
            JoinedAt = DateTime.UtcNow,
            AuthProvider = "email",
            ExternalUserId = null
        };

        _context.UserOrganizations.Add(userOrganization);
        var saveResult = await _context.SaveChangesAsync(cancellationToken);
        if (saveResult <= 0) throw new Exception("Error saving organization");

        return organization;
    }
}
