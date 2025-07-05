using Blogsphere.Api.Gateway.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blogsphere.Api.Gateway.Data.Configurations;

public class ProxyRouteConfiguration : IEntityTypeConfiguration<ProxyRoute>
{
    public void Configure(EntityTypeBuilder<ProxyRoute> builder)
    {
        builder.ToTable("Routes", "blogsphere");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.RouteId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Path)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.RateLimiterPolicy)
            .HasMaxLength(100);

        builder.Property(x => x.Methods)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions)null));

        builder.Property(x => x.Metadata)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null));

        builder.Property(x => x.ClusterId).IsRequired();

        builder.HasOne(x => x.Cluster)
            .WithMany(x => x.Routes)
            .HasForeignKey(x => x.ClusterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Headers)
            .WithOne(x => x.Route)
            .HasForeignKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Transforms)
            .WithOne(x => x.Route)
            .HasForeignKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.RouteId)
            .IsUnique();
    }
} 