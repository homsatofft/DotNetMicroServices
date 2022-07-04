using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data;

public class AppDbContext :DbContext
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  {

  }

  public DbSet<Platform> Platforms { get; set; }
  public DbSet<Command> Commands { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder
      .Entity<Platform>()
      .HasMany(p => p.Commands)
      .WithOne(c => c.Platform)
      .HasForeignKey(c => c.PlatformId);
  }
}
