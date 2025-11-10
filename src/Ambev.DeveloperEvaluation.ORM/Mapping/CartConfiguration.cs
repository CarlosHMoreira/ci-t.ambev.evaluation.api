using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.UserId).IsRequired();
        builder.Property(c => c.Date).IsRequired();

        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(c => c.Products, ownedNavigationBuilder =>
        {
            ownedNavigationBuilder.ToTable("CartItems");
            ownedNavigationBuilder.WithOwner().HasForeignKey("CartId");
            ownedNavigationBuilder.HasKey("CartId", "ProductId");

            ownedNavigationBuilder.Property(x => x.ProductId).IsRequired();
            ownedNavigationBuilder.Property(x => x.Quantity).IsRequired();

            ownedNavigationBuilder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

