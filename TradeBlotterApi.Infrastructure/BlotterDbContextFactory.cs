using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TradeBlotterApi.Infrastructure;

public sealed class BlotterDbContextFactory : IDesignTimeDbContextFactory<BlotterDbContext>
{
    public BlotterDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<BlotterDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=tradeblotter;Username=blotter;Password=blotter")
            .Options;

        return new BlotterDbContext(options);
    }
}