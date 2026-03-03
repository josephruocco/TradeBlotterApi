using Microsoft.EntityFrameworkCore;
using TradeBlotterApi.Domain.Models;

namespace TradeBlotterApi.Infrastructure;

public sealed class BlotterDbContext : DbContext
{
    public BlotterDbContext(DbContextOptions<BlotterDbContext> options) : base(options) { }

    public DbSet<Trade> Trades => Set<Trade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Trade>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Desk).HasMaxLength(32).IsRequired();
            e.Property(x => x.Product).HasMaxLength(32).IsRequired();
            e.Property(x => x.Symbol).HasMaxLength(128).IsRequired();
            e.Property(x => x.Side).HasMaxLength(16).IsRequired();
            e.Property(x => x.Counterparty).HasMaxLength(128).IsRequired();
            e.Property(x => x.Trader).HasMaxLength(128).IsRequired();
        });
    }
}