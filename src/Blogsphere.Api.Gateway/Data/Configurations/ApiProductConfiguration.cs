using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.Data.Configurations;

public class ApiProductConfiguration : IEntityTypeConfiguration<ApiProduct>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ApiProduct> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.ProductName).IsRequired();
        builder.Property(e => e.ProductDescription).IsRequired();
        
        builder.HasMany(e => e.SubscribedApis)
            .WithOne(e => e.ApiProduct)
            .HasForeignKey(e => e.ProductId);
        
        builder.HasMany(e => e.Subscriptions)
            .WithOne(e => e.ApiProduct)
            .HasForeignKey(e => e.ProductId);

        builder.HasIndex(e => e.ProductName).IsUnique();
    }
}
