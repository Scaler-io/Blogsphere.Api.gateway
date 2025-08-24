using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blogsphere.Api.Gateway.Data.Configurations;

public class ProxyClusterConfiguration : IEntityTypeConfiguration<ProxyCluster>
{
    public void Configure(EntityTypeBuilder<ProxyCluster> builder)
    {
        builder.ToTable("Clusters", "blogsphere");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClusterId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LoadBalancingPolicy)
            .HasMaxLength(50);

        builder.Property(x => x.HealthCheckPath)
            .HasMaxLength(200);

        builder.Property(x => x.IsActive).IsRequired();

        builder.HasMany(x => x.Routes)
            .WithOne(x => x.Cluster)
            .HasForeignKey(x => x.ClusterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Destinations)
            .WithOne(x => x.Cluster)
            .HasForeignKey(x => x.ClusterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ClusterId)
            .IsUnique();
    }
} 