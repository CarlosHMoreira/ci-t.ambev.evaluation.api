using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(p => p.Title).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Price).HasColumnType("numeric(18,2)").IsRequired();
        builder.Property(p => p.Description).HasMaxLength(1000).IsRequired();
        builder.Property(p => p.Category).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Image).HasMaxLength(500).IsRequired();
        
        builder.OwnsOne(p => p.Rating, r =>
        {
            r.Property(x => x.Rate).HasColumnName("RatingRate").HasColumnType("numeric(3,2)");
            r.Property(x => x.Count).HasColumnName("RatingCount");
        });
    }
}