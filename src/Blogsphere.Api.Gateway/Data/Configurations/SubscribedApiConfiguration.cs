using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blogsphere.Api.Gateway.Data.Configurations;

public class SubscribedApiConfiguration : IEntityTypeConfiguration<SubscribedApi>
{
    public void Configure(EntityTypeBuilder<SubscribedApi> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.ApiPath).IsRequired();
        builder.Property(e => e.ApiName).IsRequired();
        builder.Property(e => e.ApiDescription).IsRequired();
        
        builder.HasOne(e => e.ApiProduct)
            .WithMany(e => e.SubscribedApis)
            .HasForeignKey(e => e.ProductId);

        builder.HasIndex(e => e.ApiName).IsUnique();
        builder.HasIndex(e => e.ApiPath).IsUnique();
        
    }
}
