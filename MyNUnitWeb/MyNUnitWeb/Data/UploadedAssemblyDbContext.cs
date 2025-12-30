using Microsoft.EntityFrameworkCore;

namespace MyNUnitWeb.Data;

public class UploadedAssemblyDbContext : DbContext
{
    public UploadedAssemblyDbContext(
        DbContextOptions<UploadedAssemblyDbContext> options)
        : base(options)
    {
    }

    public DbSet<UploadedAssembly> UploadedAssemblies => this.Set<UploadedAssembly>();
}