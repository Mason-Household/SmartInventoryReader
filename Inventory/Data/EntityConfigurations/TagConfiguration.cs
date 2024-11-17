using Inventory.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Data.EntityConfigurations;

[ExcludeFromCodeCoverage]
public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        
        // Create a unique index per organization
        builder.HasIndex(x => new { x.Name, x.OrganizationId }).IsUnique();
        
        // Configure organization relationship
        builder.HasOne(x => x.Organization)
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure many-to-many relationship with Items
        builder.HasMany(x => x.Items)
            .WithMany(x => x.Tags)
            .UsingEntity<Dictionary<string, object>>(
                "ItemTags",
                j => j.HasOne<Item>().WithMany().HasForeignKey("ItemId"),
                j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId")
            );
    }
}
