using Microsoft.EntityFrameworkCore;

namespace App.Domain;

public class AppDbContext : DbContext
{
    public DbSet<Entry> Entries { get; set; } = default!;
    public DbSet<Sector> Sectors { get; set; } = default!;
    public DbSet<EntrySector> EntrySectors { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}