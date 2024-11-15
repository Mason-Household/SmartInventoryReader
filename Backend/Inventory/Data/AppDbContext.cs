using System.Linq.Expressions;
using Inventory.Domain;
using Inventory.Models;
using Inventory.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data
{
    public record AppDbContext(ICurrentUserService CurrentUserService, IApplicationBuilder DbSet<Tag> Tags, DbSet<Organization> Organizations, DbSet<InventoryItem> InventoryItems, DbSet<UserOrganization> UserOrganizations) : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
    {
        private readonly ICurrentUserService _currentUserService = CurrentUserService;
        private readonly long? _currentOrganizationId;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ICurrentUserService currentUserService) : this(default, default, default, default)
        {
            _currentUserService = currentUserService;
            _currentOrganizationId = currentUserService.GetCurrentOrganizationId();
        }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
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
}