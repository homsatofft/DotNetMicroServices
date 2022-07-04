using CommandService.SyncDataService;

namespace CommandService.Data;

public static class ContextConfiguration
{
  public static async Task PopulatePlatformsAsync(this WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var client = scope.ServiceProvider.GetRequiredService<IPlatformDataClient>();
    var platforms = await client.GetAllPlatformsAsync();
    var repo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
    foreach (var platform in platforms)
    {
      if (!await repo.ExternalPlatformExistsAsync(platform.ExternalId))
      {
        await repo.CreatePlatformAsync(platform);
      }
    }
    await repo.SaveChangesAsync();
  }
}
