using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.Number).HasMaxLength(50).IsRequired();
        builder.Property(s => s.Date).IsRequired();
        builder.Property(s => s.CustomerId).IsRequired();
        builder.Property(s => s.CustomerName).HasMaxLength(200).IsRequired();
        builder.Property(s => s.BranchId).IsRequired();
        builder.Property(s => s.BranchName).HasMaxLength(200).IsRequired();
        builder.Property(s => s.IsCancelled).IsRequired().HasDefaultValue(false);
        builder.Property(s => s.TotalAmount).HasColumnType("numeric(18,2)").IsRequired();
        
        
        builder.HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(s => s.Items, itemBuilder =>
        {
            itemBuilder.ToTable("SaleItems");
            itemBuilder.WithOwner().HasForeignKey("SaleId");
            itemBuilder.HasKey("SaleId", "ProductId");

            itemBuilder.Property(i => i.ProductId).IsRequired();
            itemBuilder.Property(i => i.ProductTitle).HasMaxLength(200).IsRequired();
            itemBuilder.Property(i => i.Quantity).IsRequired();
            itemBuilder.Property(i => i.UnitPrice).HasColumnType("numeric(18,2)").IsRequired();
            itemBuilder.Property(i => i.DiscountPercent).HasColumnType("numeric(5,2)").IsRequired();
            itemBuilder.Property(i => i.DiscountValue).HasColumnType("numeric(18,2)").IsRequired();
            itemBuilder.Property(i => i.TotalGrossAmount).HasColumnType("numeric(18,2)").IsRequired();
            itemBuilder.Property(i => i.TotalNetAmount).HasColumnType("numeric(18,2)").IsRequired();
            itemBuilder.Property(i => i.IsCancelled).IsRequired().HasDefaultValue(false);
            
            itemBuilder
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.HasIndex(s => s.Number).IsUnique();
        builder.HasIndex(s => s.Date);
        builder.HasIndex(s => s.CustomerId);
        builder.HasIndex(s => s.BranchId);
        builder.HasIndex(s => s.IsCancelled);
    }
}

