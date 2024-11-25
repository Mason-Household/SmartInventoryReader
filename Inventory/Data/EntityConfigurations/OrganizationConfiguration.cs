using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Models;

namespace Inventory.Data.EntityConfigurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Slug)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(o => o.Slug)
            .IsUnique();

        builder.Property(o => o.LogoUrl)
            .HasMaxLength(2000);

        builder.Property(o => o.SubscriptionTier)
            .HasMaxLength(50);

        // New fields configuration
        builder.Property(o => o.Domain)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(o => o.Domain);

        builder.Property(o => o.AllowedAuthProviders)
            .HasColumnType("text[]")
            .HasDefaultValue(new string[] {});

        builder.Property(o => o.AllowedEmailDomains)
            .HasColumnType("text[]")
            .HasDefaultValue(new string[] {});
    }
}
