using Microsoft.EntityFrameworkCore;
using Inventory.Models;

namespace Inventory.Data
{
    public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
    {

        public required DbSet<Item> Items { get; set; }
        public required DbSet<Category> Categories { get; set; }
        public required DbSet<PriceHistory> PriceHistory { get; set; }
        public required DbSet<Tag> Tags { get; set; }
        public required DbSet<ItemImage> ItemImages { get; set; }
        public required DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public required DbSet<Organization> Organizations { get; set; }
        public required DbSet<UserOrganization> UserOrganizations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);

            // Seed data
            modelBuilder.Entity<Organization>().HasData(
                new Organization
                {
                    Id = 1,
                    Name = "Default Organization",
                    Slug = "default-organization",
                    Domain = "default.com",
                    IsActive = true,
                    AllowedAuthProviders = new[] { "google", "email" },
                    AllowedEmailDomains = new[] { "default.com" },
                    FirebaseTenantId = "default-tenant-id"
                }
            );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = "system"; // Replace with actual user
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedBy = "system"; // Replace with actual user
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}