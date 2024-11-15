using System.Linq.Expressions;
using System.Reflection.Emit;
using Inventory.Domain;
using Inventory.Models;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly long? _currentOrganizationId;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
        _currentOrganizationId = currentUserService.GetCurrentOrganizationId();
    }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<UserOrganization> UserOrganizations { get; set; }
    protected override void OnModelCreating(ModuleBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Global query filter for multi-tenancy
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(TenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "p");
                var property = Expression.Property(parameter, nameof(TenantEntity.OrganizationId));
                var orgId = Expression.Constant(_currentOrganizationId);
                var body = Expression.Equal(property, orgId);
                var lambda = Expression.Lambda(body, parameter);

                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<TenantEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.OrganizationId = _currentOrganizationId ?? 
                    throw new UnauthorizedAccessException("Organization context is required");
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}