using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.Data.Context;

public class ProxyConfigContext(DbContextOptions<ProxyConfigContext> options) : DbContext(options)
{
    private const string DefaultSchema = "blogsphere";

    public DbSet<ProxyRoute> Routes { get; set; }
    public DbSet<ProxyCluster> Clusters { get; set; }
    public DbSet<ProxyDestination> Destinations { get; set; }
    public DbSet<ProxyHeader> Headers { get; set; }
    public DbSet<ProxyTransform> Transforms { get; set; }
    public DbSet<ApiProduct> ApiProducts { get; set; }
    public DbSet<SubscribedApi> SubscribedApis { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(DefaultSchema);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProxyConfigContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        foreach(var entry in ChangeTracker.Entries<EntityBase>())
        {
            switch(entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
} 