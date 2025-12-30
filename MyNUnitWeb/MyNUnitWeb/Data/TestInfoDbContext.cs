using Microsoft.EntityFrameworkCore;

namespace MyNUnitWeb.Data;

public class TestInfoDbContext : DbContext
{
    public TestInfoDbContext(
        DbContextOptions<TestInfoDbContext> options)
        : base(options)
    {
    }

    public DbSet<TestInfo> TestInfos => Set<TestInfo>();
}