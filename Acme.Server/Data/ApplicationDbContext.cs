using Acme.Server.Data.Records;
using Microsoft.EntityFrameworkCore;

namespace Acme.Server.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<SubscriptionRecord> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        SubscriptionRecord.OnEntityCreating(modelBuilder.Entity<SubscriptionRecord>());
    }
}
