using Inventory.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Data.EntityConfigurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
        builder.Property(x => x.ActualPrice).HasPrecision(10, 2);
        builder.Property(x => x.SuggestedPrice).HasPrecision(10, 2);
        builder.Property(x => x.Barcode).HasMaxLength(100);
        
        builder.HasIndex(x => x.Barcode).IsUnique();
        builder.HasIndex(x => x.Name);

        builder.HasOne(x => x.Category!)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Tags)
            .WithMany(x => x.Items)
            .UsingEntity<Dictionary<string, object>>(
                "ItemTags",
                j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                j => j.HasOne<Item>().WithMany().HasForeignKey("ItemId")
            );
    }
}

public interface IEntityTypeConfiguration<T>
{
}