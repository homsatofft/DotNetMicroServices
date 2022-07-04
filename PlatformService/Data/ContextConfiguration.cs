using PlatformService.Models;

namespace PlatformService.Data;

public static class ContextConfiguration
{
  public static void UseAppDbContext(this WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
    context.Seed(logger);
  }

  private static void Seed(this AppDbContext context, ILogger logger)
  {
    if (context.Platforms.Any())
    {
      return;
    }
    logger.LogInformation("--> Seeding data");
    context.Platforms.AddRange(
      new Platform { Name = ".Net", Publisher = "Microsoft", Cost = "free" },
      new Platform { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "free" },
      new Platform { Name = "Kubernetes", Publisher = "CNCF", Cost = "free" }
    );
    context.SaveChanges();
  }
}
