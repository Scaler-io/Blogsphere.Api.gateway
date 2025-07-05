using Blogsphere.Api.Gateway.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blogsphere.Api.Gateway.Data.Configurations;

public class ProxyTransformConfiguration : IEntityTypeConfiguration<ProxyTransform>
{
    public void Configure(EntityTypeBuilder<ProxyTransform> builder)
    {
        builder.ToTable("Transforms", "blogsphere");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PathPattern)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.RouteId).IsRequired();

        builder.HasOne(x => x.Route)
            .WithMany(x => x.Transforms)
            .HasForeignKey(x => x.RouteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 