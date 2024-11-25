using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Models;

namespace Inventory.Data.EntityConfigurations;

public class UserOrganizationConfiguration : IEntityTypeConfiguration<UserOrganization>
{
    public void Configure(EntityTypeBuilder<UserOrganization> builder)
    {
        builder.HasKey(uo => new { uo.UserId, uo.OrganizationId });

        builder.HasOne(uo => uo.User)
            .WithMany(u => u.Organizations)
            .HasForeignKey(uo => uo.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uo => uo.Organization)
            .WithMany()
            .HasForeignKey(uo => uo.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(uo => uo.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(uo => uo.JoinedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // New fields configuration
        builder.Property(uo => uo.AuthProvider)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(uo => uo.ExternalUserId)
            .HasMaxLength(200);

        // Add index for ExternalUserId for faster lookups
        builder.HasIndex(uo => new { uo.AuthProvider, uo.ExternalUserId });
    }
}
