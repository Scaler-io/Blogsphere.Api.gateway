using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blogsphere.Api.Gateway.Data.Configurations;

public class ProxyHeaderConfiguration : IEntityTypeConfiguration<ProxyHeader>
{
    public void Configure(EntityTypeBuilder<ProxyHeader> builder)
    {
        builder.ToTable("Headers", "blogsphere");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Values)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions)null));

        builder.Property(x => x.Mode)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(x => x.Route)
            .WithMany(x => x.Headers)
            .HasForeignKey(x => x.RouteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 