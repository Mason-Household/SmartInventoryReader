using Inventory.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Data.EntityConfigurations;

public class ConsignerPayoutConfiguration : IEntityTypeConfiguration<ConsignerPayout>
{
    public void Configure(EntityTypeBuilder<ConsignerPayout> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.PayoutDate)
            .IsRequired();

        builder.Property(p => p.PaymentMethod)
            .HasMaxLength(100);

        builder.Property(p => p.TransactionReference)
            .HasMaxLength(100);

        builder.Property(p => p.Notes)
            .HasMaxLength(500);

        builder.HasOne(p => p.Consigner)
            .WithMany(c => c.Payouts)
            .HasForeignKey(p => p.ConsignerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
