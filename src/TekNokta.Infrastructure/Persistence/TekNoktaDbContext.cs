using Microsoft.EntityFrameworkCore;
using TekNokta.Domain.Entities;

namespace TekNokta.Infrastructure.Persistence;

public sealed class TekNoktaDbContext(DbContextOptions<TekNoktaDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TekNoktaDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
