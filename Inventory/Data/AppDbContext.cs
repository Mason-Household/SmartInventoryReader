using Inventory.Models;
using Inventory.Services;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Inventory.Data
{
    [ExcludeFromCodeCoverage]
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly long? _currentOrganizationId;

        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<Organization> Organizations { get; set; } = null!;
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<PriceHistory> PriceHistories { get; set; } = null!;
        public DbSet<ItemImage> ItemImages { get; set; } = null!;
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; } = null!;
        public DbSet<UserOrganization> UserOrganizations { get; set; } = null!;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
            _currentOrganizationId = currentUserService.GetCurrentOrganizationId();
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
