using Inventory.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Inventory.Data;

[ExcludeFromCodeCoverage]
public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public required DbSet<Item> Items { get; set; }
    public required DbSet<Category> Categories { get; set; }
    public required DbSet<PriceHistory> PriceHistory { get; set; }
    public required DbSet<Tag> Tags { get; set; }
    public required DbSet<ItemImage> ItemImages { get; set; }
    public required DbSet<InventoryTransaction> InventoryTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        // Handle TenantEntity
        foreach (var entry in ChangeTracker.Entries<TenantEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.OrganizationId == default)
                {
                    throw new InvalidOperationException("OrganizationId must be set for tenant entities");
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
