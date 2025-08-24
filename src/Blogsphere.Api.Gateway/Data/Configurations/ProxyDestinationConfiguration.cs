using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blogsphere.Api.Gateway.Data.Configurations;

public class ProxyDestinationConfiguration : IEntityTypeConfiguration<ProxyDestination>
{
    public void Configure(EntityTypeBuilder<ProxyDestination> builder)
    {
        builder.ToTable("Destinations", "blogsphere");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DestinationId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.ClusterId).IsRequired();

        builder.HasOne(x => x.Cluster)
            .WithMany(x => x.Destinations)
            .HasForeignKey(x => x.ClusterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.DestinationId)
            .IsUnique();
    }
} 