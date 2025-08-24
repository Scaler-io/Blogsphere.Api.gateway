using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blogsphere.Api.Gateway.Data.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.SubscriptionKey).IsRequired();
        
        builder.HasOne(e => e.ApiProduct)
            .WithMany(e => e.Subscriptions)
            .HasForeignKey(e => e.ProductId);

        builder.HasIndex(e => e.SubscriptionKey).IsUnique();
    }
}
