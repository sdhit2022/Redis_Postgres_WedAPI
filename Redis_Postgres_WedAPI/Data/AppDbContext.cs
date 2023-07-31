using Microsoft.EntityFrameworkCore;
using Redis_Postgres_WedAPI.Models;

namespace Redis_Postgres_WedAPI.Data;

public class AppDbContext:DbContext
{
    public DbSet<Driver> Drivers { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options):
        base(options)
    {
        
    }
}
