using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class AppDbContext : DbContext
{
    public DbSet<Entry> Entries { get; set; } = default!;
    public DbSet<Sector> Sectors { get; set; } = default!;
    public DbSet<EntrySector> EntrySectors { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}