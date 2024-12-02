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

        public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService)
            : base(options)
        {
            _currentUserService = currentUserService;
            _currentOrganizationId = currentUserService.GetCurrentOrganizationId();
        }

        public DbSet<Consigner> Consigners { get; set; } = null!;
        public DbSet<ConsignerPayout> ConsignerPayouts { get; set; } = null!;
        public DbSet<Organization> Organizations { get; set; } = null!;
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<PriceHistory> PriceHistories { get; set; } = null!;
        public DbSet<ItemImage> ItemImages { get; set; } = null!;
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; } = null!;
        public DbSet<UserOrganization> UserOrganizations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Apply entity configurations
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            // Global query filter for multi-tenancy
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(TenantEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "p");
                    var property = Expression.Property(parameter, nameof(TenantEntity.OrganizationId));
                    
                    // Handle the case where _currentOrganizationId is null
                    if (_currentOrganizationId.HasValue)
                    {
                        var orgId = Expression.Constant(_currentOrganizationId.Value, typeof(long));
                        var body = Expression.Equal(property, orgId);
                        var lambda = Expression.Lambda(body, parameter);
                        builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                    }
                    else
                    {
                        // When no organization is set, return false (no records)
                        var body = Expression.Constant(false);
                        var lambda = Expression.Lambda(body, parameter);
                        builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                    }
                }
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = _currentUserService.GetUserEmail() ?? "system";
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedBy = _currentUserService.GetUserEmail() ?? "system";
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}