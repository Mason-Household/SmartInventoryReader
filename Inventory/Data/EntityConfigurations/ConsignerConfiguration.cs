using Inventory.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Data.EntityConfigurations;

public class ConsignerConfiguration : IEntityTypeConfiguration<Consigner>
{
    public void Configure(EntityTypeBuilder<Consigner> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Email)
            .HasMaxLength(256);

        builder.Property(c => c.Phone)
            .HasMaxLength(50);

        builder.Property(c => c.PaymentDetails)
            .HasMaxLength(500);

        builder.Property(c => c.CommissionRate)
            .HasPrecision(4, 2);

        builder.Property(c => c.UnpaidBalance)
            .HasPrecision(18, 2);

        builder.Property(c => c.TotalPaidOut)
            .HasPrecision(18, 2);

        builder.HasMany(c => c.Items)
            .WithOne(i => i.Consigner)
            .HasForeignKey(i => i.ConsignerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Payouts)
            .WithOne(p => p.Consigner)
            .HasForeignKey(p => p.ConsignerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
