using Microsoft.EntityFrameworkCore;
using SchedulePlanner.Backend.Data.Models;

namespace SchedulePlanner.Backend.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
    : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Session> Sessions { get; set; } = null!;
     
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityColumns();

        modelBuilder.Entity<User>()
            .HasMany(e => e.Sessions)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId);
        
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("Database"));
    }
}